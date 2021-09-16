using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController : BaseApiContrroler
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public DataContext _context { get; }
        public AccountController(DataContext context, ITokenService tokenService,
        IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(registerDTO registrDto)
        {
            if (await userExist(registrDto.UserName)) return BadRequest("Username is Taken");

            var user = _mapper.Map<AppUser>(registrDto);

            // using statment is for desposed method in the hashind class
            using var hmac = new HMACSHA512();


            user.UserName = registrDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registrDto.password));
            user.PasswordSalt = hmac.Key;


            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.creatToken(user),
                KnownAs=user.KnownAs
            };

        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(loginDTO loginUser)
        {
            var user = await _context.Users
                .Include(p => p.Photos)
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
                Token = _tokenService.creatToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs=user.KnownAs
            };


        }
        private async Task<bool> userExist(string UserName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == UserName.ToLower());
        }


    }


}