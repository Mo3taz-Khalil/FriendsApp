using API.Data;
using API.Helpers;
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
            IConfiguration config)
          {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));//name of sction in app seeting.json
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext<DataContext>(n =>
            {
                n.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            return services;
          }
    }
}