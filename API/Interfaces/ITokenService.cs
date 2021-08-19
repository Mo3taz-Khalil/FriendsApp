using API.Entities;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string creatToken(AppUser user);
    }
}