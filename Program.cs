using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using ShortLink.Data;
using Microsoft.AspNetCore.Identity;
using Prometheus;

namespace ShortLink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. SERVISLER (Services)
            // ==========================================

            // Veritabanı Bağlantısı
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                   throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Identity (Kullanıcı Yönetimi)
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // MVC ve Razor
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // ==========================================
            // 2. ARA KATMANLAR (Middleware) - SIRALAMA KRİTİK!
            // ==========================================

            // [KRİTİK 1] Forwarded Headers: IP ve Domain düzeltmesi için EN BAŞTA olmalı
            var forwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            };
            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeaderOptions);

            // Hata Yönetimi
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // [KRİTİK 2] Prometheus Metrikleri (Routing'den sonra)
            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            // ==========================================
            // 3. ROTALAR (Endpoints)
            // ==========================================

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            // [KRİTİK 3] Metrik Endpoint'i
            app.MapMetrics();

            // [KRİTİK 4] Otomatik Migration (Veritabanı)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Veritabanı migration hatası.");
                }
            }

            // [KRİTİK 5] Kısa Link Yakalayıcı (En sonda)
            app.MapFallbackToController("RedirectToUrl", "Home");

            app.Run();
        }
    }
}