using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TechWizProject.Models;
using TechWizProject.Areas.Admin.Services;
using TechWizProject.Areas.Users.Services;
using TechWizProject.Services;
using IOrderService = TechWizProject.Services.IOrderService;

namespace TechWizProject
{
    public class Startup
    {
        private readonly IConfiguration iConfiguration;

        public Startup(IConfiguration _iConfiguration)
        {
            iConfiguration = _iConfiguration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddSession();
            var connectionString = iConfiguration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DatabaseContext>(option =>
              option.UseLazyLoadingProxies().UseSqlServer(connectionString));
            services.AddDbContext<Areas.Admin.Models.DatabaseContext>(option =>
                option.UseLazyLoadingProxies().UseSqlServer(connectionString));

            services.AddScoped<IRestaurantsServies, RestaurantsServicesImpl>();
            
            services.AddScoped<IFoodService, FoodServiceImpl>();
            services.AddScoped<IFoodsService, FoodsService>();
            services.AddScoped<ICartService, CartServiceImpl>();
            services.AddScoped<IUserService, UserServiceImpl>();
            services.AddScoped<IOrderService, OrderServiceImpl>();
            services.AddScoped<CategoryService, CategoryServiceImpl>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<Areas.Users.Services.IOrderService, IOrderServiceImpl>();
            services.AddScoped<TagService, TagServiceImpl>();
            services.AddScoped<OrderTrackingService, OrderTrackingServiceImpl>();
            services.AddScoped<IFoodRatingService, FoodRatingServiceImpl>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=home}/{action=index}/{id?}"
                );
            });
        }
    }
}
