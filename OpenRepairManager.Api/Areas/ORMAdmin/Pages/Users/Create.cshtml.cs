using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenRepairManager.Api.Data.Models;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Users;

public class CreateModel : PageModel
{
    private readonly UserManager<ORMUser> _userManager;

    public CreateModel(UserManager<ORMUser> userManager)
    {
        _userManager = userManager;
    }
    
    [BindProperty]
    public string Name { get; set; }
    
    [BindProperty]
    public string Email { get; set; }
    
    [BindProperty]
    public string Password { get; set; }
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ORMUser User = new ORMUser()
        {
            UserName = Email,
            Email = Email,
            Name = Name,
            EmailConfirmed = true
        };
        
        IdentityResult result = await _userManager.CreateAsync(User,Password);
        if (result.Succeeded)
        {
           return RedirectToPage("Index");
        }
        return Page();
    }
}