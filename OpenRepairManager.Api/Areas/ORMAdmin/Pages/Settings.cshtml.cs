using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Services;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages;

[BindProperties]
public class Settings : PageModel
{
    
    public string GMapsKey { get; set; }
    [Required]
    public string SiteName { get; set; }
    
    public bool status { get; set; }
    
    public void OnGet()
    {
        var GmapsKey = SettingsService.GetSetting("GMapsKey");
        if (GmapsKey != null)
        {
            GMapsKey = GmapsKey.Value;
        }
        SiteName = SettingsService.GetSetting("SiteName").Value;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        SettingsService.AddOrUpdate(new()
        {
            Name = "SiteName",
            Value = SiteName
        });
        if (!string.IsNullOrEmpty(GMapsKey))
        {
            SettingsService.AddOrUpdate(new()
            {
                Name = "GMapsKey",
                Value = GMapsKey
            });
        }
        status = true;
        return Page();
    }
}