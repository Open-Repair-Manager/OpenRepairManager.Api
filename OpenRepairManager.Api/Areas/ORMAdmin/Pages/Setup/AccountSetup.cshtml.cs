using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Data.Models;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Setup;

[AllowAnonymous]
[BindProperties]
public class AccountSetup : PageModel
{
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    
    public AccountSetup(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ORMUser user = new ORMUser()
        {
            UserName = Username,
            Email = Username,
            EmailConfirmed = true
        };
        IdentityResult result = await _userManager.CreateAsync(user, Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, true);
            return RedirectToPage("SetupComplete");
        }
        ViewData["Error"] = result.Errors.First().Description;
        return Page();
    }
}