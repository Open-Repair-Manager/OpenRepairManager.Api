using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Common.Models;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Locations
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Location Location { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Location = await _context.Location.FirstOrDefaultAsync(m => m.LocationID == id);

            if (Location == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
