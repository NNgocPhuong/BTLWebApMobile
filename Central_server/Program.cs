using Central_server.Data;
using Central_server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Central_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<FarmTrackingContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("FarmTrackingDB")));

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Add authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Users/Login";
                    options.LogoutPath = "/Users/Logout";
                    options.AccessDeniedPath = "/AccessDenied";
                    // Thiết lập cookie sống trong 1 ngày
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);

                    // Ghi nhớ đăng nhập, cookie không bị xóa khi trình duyệt đóng
                    options.SlidingExpiration = true; // Tự động gia hạn thời gian nếu người dùng tương tác
                    options.Cookie.IsEssential = true; // Đảm bảo cookie hoạt động kể cả khi tuân thủ GDPR
                });

            // Add session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1); // Session sống trong 1 ngày
                options.Cookie.HttpOnly = true; // Bảo mật cookie
                options.Cookie.IsEssential = true; // Đảm bảo hoạt động ngay cả với GDPR
            });

            // Register the hosted service
            builder.Services.AddHostedService<StationDataFetcher>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection(); // Redirect http to https
            app.UseStaticFiles(); // Serves static files and short-circuits the pipeline, so to speak

            app.UseRouting(); // Adds routing to the request pipeline

            app.UseCors(); // Use CORS

            app.UseSession(); // Use session

            app.UseCookiePolicy(); // Use cookie policy

            app.UseAuthentication(); // Use authentication

            app.UseAuthorization(); // Use authorization

            app.MapControllerRoute( // Adds endpoints for controller actions to the IEndpointRouteBuilder without requiring attribute routes
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

