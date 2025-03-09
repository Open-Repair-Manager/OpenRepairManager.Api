using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Data;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.ApiKey
{
    public class RevokeModel : PageModel
    {

        private readonly ApplicationDbContext _context;

        public RevokeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Common.Models.ApiKey key { get; set; }

        public async Task<IActionResult> OnGetAsync(string? _key)
        {
            if (_key == null) 
            {
                return NotFound();
            }
            
            key = await _context.ApiKey.Where(k => k.Key == _key).FirstOrDefaultAsync();
            
            if (key == null)
            {
                return NotFound();
            }

            return Page();

        }

        public async Task<IActionResult> OnPostAsync(string? _key)
        {
            if (_key == null)
            {
                return NotFound();
            }

            key = await _context.ApiKey.Where(k => k.Key == _key).FirstOrDefaultAsync();

            if (key != null)
            {
                _context.ApiKey.Remove(key);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }

    }
}
