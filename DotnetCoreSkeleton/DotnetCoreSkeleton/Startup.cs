using AutoMapper;
using DAL;
using DAL.Repository;
using DotnetCoreSkeleton.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace DotnetCoreSkeleton
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

            services.AddControllers();

            services.AddDbContext<MyDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //AutoMapper DI

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mappers.AutoMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //AutoMapper DI End

            //REPOSITORY DI
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            //AppServices DI
            //Inject your services here

            //Cors DI
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options =>
                options.AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200", "http://sandbox.aimviz.com:6333", "http://sandbox:6333"));
            });
            services.AddSignalR();

            //JWT DI
            var key = Encoding.ASCII.GetBytes(Configuration["Authentication:JwtBearer:SecurityKey"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,

                // Allow to use seconds for expiration of token
                // Required only when token lifetime less than 5 minutes
                // THIS ONE
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Authentication:JwtBearer:Issuer"],
                        ValidAudience = Configuration["Authentication:JwtBearer:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:JwtBearer:SecurityKey"]))
                    };
                });

            //JWT DI END

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ActiveUserPolicy", policy => policy.Requirements.Add(new ActiveUserAuthorizationRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotnetCoreSkeleton", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotnetCoreSkeleton v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
