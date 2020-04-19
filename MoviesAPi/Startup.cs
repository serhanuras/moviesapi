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


            // services.AddDbContext<ApplicationDbContext>(options =>
            // {
            //     options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MoviesAPi"));
            // });

            ConnectString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(ConnectString,  b => b.MigrationsAssembly("MoviesAPi"));
            });

           

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddSingleton<IRepository, InMemoryRepository>();
            //services.AddScoped<IRepository, EFCoreRepository>();
            services.AddTransient<MyActionFilter>();
            //services.AddSingleton<IHostedService, WriteToFileHostedService>();
            // services.AddHostedService<WriteToFileHostedService>();
            services.AddHostedService<HelloWorldHostedService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IFileStorageService,InAppStorageService>();

            services.AddHttpContextAccessor();

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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
