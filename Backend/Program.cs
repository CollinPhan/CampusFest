
using Backend.API;
using Backend.API.Filters;
using Backend.API.Middleware.ExceptionHandler;
using Backend.API.Services.Implementation;
using Backend.API.Services.Interface;
using Backend.Cores.Commons;
using Backend.Cores.Exceptions;
using Backend.Infrastructures.Data;
using Backend.Infrastructures.Repositories.Implementation;
using Backend.Infrastructures.Repositories.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Setup Cors
            builder.Services.AddCors(options =>
                options.AddPolicy("defaultPolicy",
                    policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                )
            );

            // Add Exception Handler
            builder.Services.AddExceptionHandler<APIExceptionHandler>();

            // Add Automapper
            builder.Services.AddAutoMapper(typeof(EntityMapper));

            // Add Repositories
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepositoy<>));

            // Add Services
            builder.Services.AddServices();

            // Add Database Context
            builder.Services.AddDbContext<CampusFestDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(connectionString: builder.Configuration.GetConnectionString("default")));

            // Add Authentication Scheme
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddControllers(config =>
            {
                config.Filters.Add<ActionExceptionFilter>();
            });

            // Configuring Services
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "This is authentication scheme using Jwt Token. The token location will be in the header",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                   {
                     new OpenApiSecurityScheme{ Reference = new OpenApiReference {Id = "Bearer", Type = ReferenceType.SecurityScheme} },
                     new string[] { }
                   }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("defaultPolicy");

            app.UseHttpsRedirection();

            app.UseExceptionHandler( _ => { });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
