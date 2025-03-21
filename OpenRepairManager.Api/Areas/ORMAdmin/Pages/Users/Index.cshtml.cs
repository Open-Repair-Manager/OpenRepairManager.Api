using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using OpenRepairManager.Api.Data.Models;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Users;

public class Index : PageModel
{
    private readonly UserManager<ORMUser> _userManager;
    
    public IList<ORMUser> Users { get; set; }
    
    public Index(UserManager<ORMUser> userManager)
    {
        _userManager = userManager;
    }
    
    public void OnGet()
    {
        Users = _userManager.Users.ToList();
    }
}