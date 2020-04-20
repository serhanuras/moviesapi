using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesAPi.Entities;
using MoviesAPi.Extensions;
using MoviesAPi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using MoviesAPi.Filters;
using Microsoft.EntityFrameworkCore;
using MoviesAPi.PostgreSqlProvider;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MoviesAPi
{
    public class Startup
    {

        public static string ConnectString { get; set; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //adding filter globally...
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
            })
            .AddNewtonsoftJson()
            .AddXmlDataContractSerializerFormatters();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAPIRequestIO",
                    builder => builder.WithOrigins("https://www.apirequest.io").WithMethods("GET", "POST").AllowAnyHeader());
            });



            // services.AddDbContext<ApplicationDbContext>(options =>
            // {
            //     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MoviesAPi"));
            // });

            ConnectString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(ConnectString, b => b.MigrationsAssembly("MoviesAPi"));
            });



            services.AddResponseCaching();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddSingleton<IRepository, InMemoryRepository>();
            //services.AddScoped<IRepository, EFCoreRepository>();
            services.AddTransient<MyActionFilter>();
            //services.AddSingleton<IHostedService, WriteToFileHostedService>();
            // services.AddHostedService<WriteToFileHostedService>();
            services.AddHostedService<HelloWorldHostedService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IFileStorageService, InAppStorageService>();
            services.AddIdentity<ApplicationUser, IdentityRole<long>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<HashService>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.Configure<IdentityOptions>(options =>
                        {
                            // Password settings
                            options.Password.RequireDigit = true;
                            options.Password.RequiredLength = 8;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = true;
                            options.Password.RequireLowercase = false;

                            // Lockout settings
                            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                            options.Lockout.MaxFailedAccessAttempts = 10;

                            // // Cookie settings
                            // options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                            // options.Cookies.ApplicationCookie.LoginPath = "/Account/Login";

                            // User settings
                            options.User.RequireUniqueEmail = true;
                        });

            services.AddHttpContextAccessor();


            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", 
                    
                    Title = "MoviesAPI",
                    Description = "This is a Web API for Movies operations",
                    License = new OpenApiLicense()
                    {
                        Name = "MIT"
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "",
                        Email = "",
                        Url = new Uri("http://www.github.com")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

            });

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseRequestResponseLogging();

            // app.Use(async (context, next) =>
            // {


            //     using (var swapStream = new MemoryStream())
            //     {
            //         context.Request.EnableBuffering();

            //         await context.Request.Body.CopyToAsync(swapStream);

            //         string requestBody = ReadStreamInChunks(swapStream);

            //         logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
            //                $"Schema:{context.Request.Scheme} " +
            //                $"Host: {context.Request.Host} " +
            //                $"Path: {context.Request.Path} " +
            //                $"QueryString: {context.Request.QueryString} " +
            //                $"Request Body: {requestBody}");


            //         context.Request.Body.Position = 0;
            //     }

            //     using (var swapStream = new MemoryStream())
            //     {
            //         var originalResponseBody = context.Response.Body;
            //         context.Response.Body = swapStream;

            //         await next.Invoke();

            //         swapStream.Seek(0, SeekOrigin.Begin);
            //         string responseBody = new StreamReader(swapStream).ReadToEnd();
            //         swapStream.Seek(0, SeekOrigin.Begin);



            //         await swapStream.CopyToAsync(originalResponseBody);
            //         context.Response.Body = originalResponseBody;

            //         logger.LogInformation(responseBody);
            //     }
            // });

            app.Map("/map1", (app) =>
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Middleware page...");
                });
            });


             app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI");
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.IsDevelopment())
            {
                app.UseCors();
                // This policy would be applied at the Web API level
                //app.UseCors(builder => 
                //builder.WithOrigins("https://www.apirequest.io").WithMethods("GET", "POST").AllowAnyHeader());
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
