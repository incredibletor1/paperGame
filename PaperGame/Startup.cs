using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaperGame.Middlewares.Exceptions;
using PaperGame.Models.Context;
using PaperGame.Models.Context.Interface;
using PaperGame.Repositories;
using PaperGame.Repositories.Interfaces;
using PaperGame.Services;
using PaperGame.Services.Interfaces;
using PaperGame.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PaperGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            Log.Information("Start configure services");
            try
            {
                ConfigureServicesWithLoggerActivated(services);
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e.Message);
            }
        }

        public void ConfigureServicesWithLoggerActivated(IServiceCollection services)
        {
            services.AddDbContext<PaperGameContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PaperGameDbConnection"))
            );

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(30);
                hubOptions.HandshakeTimeout = TimeSpan.FromMinutes(1);
                hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(2);////////////FROM MINUTES 1////////////////

                //hubOptions.StreamBufferCapacity = 100;

            });

            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration.GetSection("Authentication:Google:ClientId").ToString();
                    googleOptions.ClientSecret = Configuration.GetSection("Authentication:Google:ClientSecret").ToString();
                }); 


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaperGame_Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = @"Please insert JWT.
                                    Enter 'Bearer' [space] and then your token in the text input below.
                                    Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
            });

            services.AddAutoMapper(typeof(Startup));

            // Register EF context
            services.AddScoped<IPaperGameContext, PaperGameContext>();

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGameRoomRepository, GameRoomRepository>();

            // Register Services
            services.AddScoped<IGameRoomService, GameRoomService>();
            services.AddScoped<IUserService, UserService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaperGame_Api v1"));
            }

            app.UseExceptionHandlerMiddleware();
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("ClientPermission");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PaperGameHub>("/hubs/papergame", options => 
                {
                    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                });
            });
        }
    }
}
