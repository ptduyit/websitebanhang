using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Hubs;
using WebsiteBanHang.Models;

namespace WebsiteBanHang
{
    public class Startup
    {
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

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var secretAppsetting = Configuration.GetSection(nameof(JwtSecretKey));

            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretAppsetting[nameof(JwtSecretKey.SecretKey)]));

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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions( options => {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    //options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
                });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;//off auto 400 response
            });
            services.AddSignalR();

            services.Configure<FacebookAuthSettings>(Configuration.GetSection("FacebookAuthSettings"));
            services.Configure<GoogleAuthSettings>(Configuration.GetSection("GoogleAuthSettings"));
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

            app.UseStaticFiles(); // For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = "/Resources"
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<EchoHub>("/echo");
            });
            SqlDependency.Start("Server=.\\SQLEXPRESS;Database=SaleDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            app.UseMvc();
            CreateRoles(serviceProvider);
        }
        private void CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            //var saleDBContext = serviceProvider.GetRequiredService<SaleDBContext>();
            string[] roleNames = { "admin", "employee", "member" };
            Task<IdentityResult> roleResult;

            foreach (var roleName in roleNames)
            {
                Task<bool> roleExist = RoleManager.RoleExistsAsync(roleName);
                roleExist.Wait();
                if (!roleExist.Result)
                {
                    roleResult = RoleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    roleResult.Wait();
                }
            }

            //var poweruser = new User
            //{
            //    UserName = "admin@gmail.com",//Configuration["UserSettings:UserEmail"],
            //    Email = "admin@gmail.com"
            //};
            //string userPWD = "123123123";//Configuration["UserSettings:UserPassword"];
            //Task<User> _user = UserManager.FindByEmailAsync("admin@gmail.com");//Configuration["UserSettings:UserEmail"]);
            //_user.Wait();

            //if (_user.Result == null)
            //{
            //    Task<IdentityResult> createPowerUser = UserManager.CreateAsync(poweruser, userPWD);
            //    if (createPowerUser.Result.Succeeded)
            //    {
            //        //here we tie the new user to the role
            //        Task<IdentityResult> newUserRole = UserManager.AddToRoleAsync(poweruser, "Admin");
            //        newUserRole.Wait();
            //        Task<User> user = UserManager.FindByEmailAsync("admin@gmail.com");
            //        user.Wait();
            //        Guid UserId = user.Result.Id;
            //        saleDBContext.Add(new UserInfo
            //        {
            //            UserId = UserId,
            //            FullName = "admin",
            //            BirthDate = DateTime.Now
            //        });
            //        saleDBContext.SaveChanges();
            //    }
            //}
        }
    }
}
