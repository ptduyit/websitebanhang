﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebsiteBanHang.Auth;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;

namespace WebsiteBanHang
{
    public class Startup
    {
        private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SaleDBContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<User>().AddRoles<IdentityRole>().AddEntityFrameworkStores<SaleDBContext>();
            //services.AddIdentityCore<User>()
            //    .AddEntityFrameworkStores<SaleDBContext>()
            //    .AddDefaultUI()
            //    .AddDefaultTokenProviders();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            

            // api user claim policy
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            //});
            services.AddSingleton<IJwtFactory, JwtFactory>();

            // add identity
            var builder = services.AddDefaultIdentity<User>(o =>
             {
                 // configure identity options
                 o.Password.RequireDigit = false;
                 o.Password.RequireLowercase = false;
                 o.Password.RequireUppercase = false;
                 o.Password.RequireNonAlphanumeric = false;
                 o.Password.RequiredLength = 6;
             });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole<Guid>), builder.Services);
            builder.AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<SaleDBContext>().AddDefaultTokenProviders();
            //builder.AddRoleValidator<RoleValidator<IdentityRole>>();
            //builder.AddRoleManager<RoleManager<IdentityRole>>();
            //builder.AddSignInManager<SignInManager<User>>();
            services.AddAutoMapper();
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            corsBuilder.AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder.Build());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("AllowAll");
            app.UseMvc();
            CreateRoles(serviceProvider);
        }
        private void CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            string[] roleNames = { "Admin", "Manager", "Member" };
            Task<IdentityResult> roleResult;

            foreach (var roleName in roleNames)
            {
                Task<bool> roleExist = RoleManager.RoleExistsAsync(roleName);
                roleExist.Wait();
                if (!roleExist.Result)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = RoleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    roleResult.Wait();
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new User
            {

                UserName = "nhotuoitho97@gmail.com",//Configuration["UserSettings:UserEmail"],
                Email = "nhotuoitho97@gmail.com"
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = "123123123";//Configuration["UserSettings:UserPassword"];
            Task<User> _user = UserManager.FindByEmailAsync("nhotuoitho97@gmail.com");//Configuration["UserSettings:UserEmail"]);
            _user.Wait();

            if (_user.Result == null)
            {
                Task<IdentityResult> createPowerUser = UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Result.Succeeded)
                {
                    //here we tie the new user to the role
                    Task<IdentityResult> newUserRole = UserManager.AddToRoleAsync(poweruser, "Admin");
                    newUserRole.Wait();

                }
            }
        }
    }
}