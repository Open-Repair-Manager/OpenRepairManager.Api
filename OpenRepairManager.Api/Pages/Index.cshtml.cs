using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Services;

namespace OpenRepairManager.Api.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var isFirstRun = SettingsService.GetSetting("FirstRun");

        if (isFirstRun.Value == "yes")
        {
            return Redirect("/ORMAdmin/Setup");
        }
        
        return Redirect("/ORMAdmin/Index");
    }
}