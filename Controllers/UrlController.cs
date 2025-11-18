using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortLink.Data;
using ShortLink.Models;
using System.Security.Claims;

namespace ShortLink.Controllers
{
    [Authorize]
    public class UrlController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UrlController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var userLinks = await _context.ShortenedUrls
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.CreatedAt) 
                .ToListAsync();

            return View(userLinks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string originalUrl)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Eğer link http veya https ile başlamıyorsa, başına https:// ekle
            if (!originalUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !originalUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                originalUrl = "https://" + originalUrl;
            }

            var shortCode = GenerateShortCode();

            var newLink = new ShortenedUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            _context.ShortenedUrls.Add(newLink);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Silinecek linki veritabanında bul
            var link = await _context.ShortenedUrls.FindAsync(id);

            if (link == null)
            {
                return NotFound(); // Link yoksa hata ver
            }

            // 2. GÜVENLİK KONTROLÜ: Linkin sahibi şu anki kullanıcı mı?
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (link.UserId != userId)
            {
                return Unauthorized(); // Başkasının linkini silmeye çalışıyorsa durdur!
            }

            // 3. Silme işlemini yap
            _context.ShortenedUrls.Remove(link);
            await _context.SaveChangesAsync();

            // 4. Dashboard'a geri dön
            return RedirectToAction(nameof(Index));
        }
        private string GenerateShortCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        }
    }
}