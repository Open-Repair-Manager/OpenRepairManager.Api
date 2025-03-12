using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Data;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages;

public class Index : PageModel
{
    private readonly ApplicationDbContext _context;
    
    public int SessionCount { get; set; }
    public int TotalItems { get; set; }
    public int TotalRepaired { get; set; }
    
    public Index(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        SessionCount = _context.Session.Count(m => m.SessionDate <= DateTime.Now);
        TotalItems = _context.RepairItem.Count();
        TotalRepaired = _context.RepairItem.Count(m => m.RepairStatus == 1);
    }
}