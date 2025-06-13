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
    
    public string Name { get; set; }
    
    private readonly UserManager<ORMUser> _userManager;
    private readonly SignInManager<ORMUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public AccountSetup(UserManager<ORMUser> userManager, SignInManager<ORMUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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
            Name = Name,
            EmailConfirmed = true
        };
        IdentityResult result = await _userManager.CreateAsync(user, Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            await _signInManager.SignInAsync(user, true);
            return RedirectToPage("SetupComplete");
        }
        ViewData["Error"] = result.Errors.First().Description;
        return Page();
    }
}