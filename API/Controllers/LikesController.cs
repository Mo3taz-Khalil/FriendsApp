using API.DTOs;
using API.Entities;
using API.Exetentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiContrroler
    {
        private readonly IUserRepository _UserRepository;
        private readonly ILikesRepositry _LikesRepositry;

        public LikesController(IUserRepository userRepository, ILikesRepositry likesRepositry)
        {
            _UserRepository = userRepository;
            _LikesRepositry = likesRepositry;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> addLike(string username)
        {
            var message = "";
            var sourceId = User.GetUserId();
            var likedUser = await _UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _LikesRepositry.GetUserWithLikes(sourceId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You can not like yourself");

            var userlike = await _LikesRepositry.GetUserLike(sourceId, likedUser.Id);

            if (userlike != null)
            {
                _LikesRepositry.DeletUserLike(userlike);
                message = "You unliked ";

            }
            else
            {
                userlike = new UserLike
                {
                    SourceUserId = sourceId,
                    LikedUserId = likedUser.Id
                };
                message = "You liked ";

            }

            sourceUser.LikedByMe.Add(userlike);
            if (await _UserRepository.SaveAllAsync()) return Ok(message);

            return BadRequest("Failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> getUsersLike([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _LikesRepositry.GetUserLikes(likesParams);
            Response.AddPaginationHeaders(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }







    }
}
