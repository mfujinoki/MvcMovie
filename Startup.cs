using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using Microsoft.Extensions.Logging;
namespace MvcMovie
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
            services.AddMvc();
            services.AddDbContext<MvcMovieContext>(options =>
            options.UseSqlite("Data Source=MvcMovie.db"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            using (var scope = app.ApplicationServices.CreateScope()) 
            { 
                        var services = scope.ServiceProvider;

                    try 
                    {
                        DBinitialize.EnsureCreated(services); 
                        SeedData.Initialize(services);
                    } 
                    catch (Exception ex) 
                    { 
                            var logger = services.GetRequiredService<ILogger<Program>>();                                
                            logger.LogError(ex, "An error occurred seeding the DB."); 
                    }
            }

        }
    }
}
