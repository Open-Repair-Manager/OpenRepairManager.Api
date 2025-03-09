using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Services;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Setup
{
    [AllowAnonymous]
    public class SetupCompleteModel : PageModel
    {

        public IActionResult OnGet()
        {
            var setting = SettingsService.GetSetting("firstrun");
            if (setting != null)
            {
                if (setting.Value != "yes")
                {
                    return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            SettingsService.AddOrUpdate(new()
            {
                Name = "firstrun",
                Value = "no"
            });
            return Redirect("/ORMAdmin/");
        }
    }
}
