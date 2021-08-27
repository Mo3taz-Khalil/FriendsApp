using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController : BaseApiContrroler
    {
        private readonly ITokenService _tokenService;

        public DataContext _context { get; }
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(registerDTO userData)
        {
            if (await userExist(userData.UserName)) return BadRequest("Username is Taken");

            // using statment is for desposed method in the hashind class
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = userData.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userData.password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.creatToken(user)
            };

        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(loginDTO loginUser)
        {
            var user = await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == loginUser.UserName);

            if (user == null) return Unauthorized("Invalid User Name");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var cmputedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUser.password));

            for (int i = 0; i < cmputedhash.Length; i++)
            {
                if (cmputedhash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

           return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.creatToken(user)
            };


        }
        private async Task<bool> userExist(string UserName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == UserName.ToLower());
        }


    }


}