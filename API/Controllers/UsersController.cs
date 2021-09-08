using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiContrroler
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
        }
        // GET: api/Users/username
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetAppUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);

            return _mapper.Map<MemberDto>(user);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //get the user name from the token claim the api uses to authenticate this user

            var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user =await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto,user);

            _userRepository.update(user);
            if(await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
           
        }


    }
}
