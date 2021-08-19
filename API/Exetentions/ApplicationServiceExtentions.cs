using API.Data;
using API.Interfaces;
using API.Servises;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Exetentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration Config)
          {
            services.AddScoped<ITokenService,TokenService>();


            services.AddDbContext<DataContext>(n =>
            {
                n.UseSqlServer(Config.GetConnectionString("DefaultConnection"));
            });

            return services;
          }
    }
}