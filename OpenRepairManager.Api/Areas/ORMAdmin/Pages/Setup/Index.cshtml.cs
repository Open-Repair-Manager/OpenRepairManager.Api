using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Common.Models;
using OpenRepairManager.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Setup
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage="Please enter a name")]
        public string Name { get; set; }

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
            if(!ModelState.IsValid)
            {
                return Page();
            }
            Setting setting = new()
            {
                Name = "SiteName",
                Value = Name
            };
            bool success = SettingsService.AddOrUpdate(setting);
            if (!success)
            {
                return Page();
            }
            return RedirectToPage("DatabaseSetup");
        }
    }
}
