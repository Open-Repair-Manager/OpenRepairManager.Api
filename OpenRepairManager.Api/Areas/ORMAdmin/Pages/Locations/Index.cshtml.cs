using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Services;
using OpenRepairManager.Common.Models;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Locations
{
    public class IndexModel : PageModel
    {
        
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }
        

        public IList<Location> Location { get;set; }

        public async Task OnGetAsync()
        {
            Location = await _context.Location.ToListAsync();
        }
    }
}
