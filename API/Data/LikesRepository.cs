using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Exetentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepositry
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public void DeletUserLike(UserLike userlike)
        {
           _context.Likes.Remove(userlike);          
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u=>u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.predicate=="likedByMeToUsers")
            {
                likes=likes.Where(l=>l.SourceUserId==likesParams.UserId);
                users = likes.Select(like=>like.LikedUser);
            }
            if(likesParams.predicate=="likedByUsersToMe")
            {
                likes=likes.Where(l=>l.LikedUserId==likesParams.UserId);
                users = likes.Select(like=>like.SourceUser);
            }

            var likedUsers= users.Select(user=> new LikeDto
            {
                Username=user.UserName,
                KnownAs=user.KnownAs,
                Age=user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City=user.City,
                Id=user.Id
            });

            return await PagedList<LikeDto>.CreatAsync(likedUsers,likesParams.PageNumber,likesParams.pageSize);

        }
        //list Of Users that this user Has liked 
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x=>x.LikedByMe)
                .FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}