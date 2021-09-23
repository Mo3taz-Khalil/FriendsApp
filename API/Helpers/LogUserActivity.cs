using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exetentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var result = await next();
            if (!result.HttpContext.User.Identity.IsAuthenticated) return;

            var id = result.HttpContext.User.GetUserId();

            var repo = result.HttpContext.RequestServices.GetService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(id);
            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync();


        }
    }
}