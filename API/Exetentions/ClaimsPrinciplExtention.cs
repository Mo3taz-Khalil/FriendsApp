using System.Security.Claims;

namespace API.Exetentions
{
    public static class ClaimsPrinciplExtention
    {
        public static string GetUserName(this ClaimsPrincipal user){
            return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}