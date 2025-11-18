using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Veritabaný iþlemleri için
using ShortLink.Data;
using ShortLink.Models;
using System.Diagnostics;

namespace ShortLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Veritabaný baðlantýsý

        // Constructor'da veritabanýný içeri alýyoruz (Dependency Injection)
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- SÝHÝRLÝ YÖNLENDÝRME METODU ---
        public async Task<IActionResult> RedirectToUrl()
        {
            // Adres çubuðundan kodu al
            var shortCode = Request.Path.Value?.TrimStart('/');

            if (string.IsNullOrEmpty(shortCode))
            {
                return RedirectToAction(nameof(Index));
            }

            // Veritabanýnda ara
            var shortLink = await _context.ShortenedUrls
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (shortLink == null)
            {
                return NotFound(); // Link yoksa 404 ver veya anasayfaya at
            }

            // --- TIKLANMA SAYISINI ARTIR ---
            shortLink.ClickCount++;
            _context.ShortenedUrls.Update(shortLink);
            await _context.SaveChangesAsync();

            // --- GERÇEK YÖNLENDÝRME ---
            return Redirect(shortLink.OriginalUrl);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}