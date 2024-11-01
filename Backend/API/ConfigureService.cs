using Backend.API.Services.Implementation;
using Backend.API.Services.Interface;

namespace Backend.API
{
    public static class ConfigureService
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAccountService,AccountService>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped<ICampusService, CampusService>();
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IEventService, EventService>();

            return services;
        }
    }
}
