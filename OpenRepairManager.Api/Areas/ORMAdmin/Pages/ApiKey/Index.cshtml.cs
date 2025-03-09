using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Service;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Common.Models;
using System.Security.Claims;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.ApiKey
{
    public class IndexModel : PageModel
    {
        private ApiKeyService apiKeyService = new ApiKeyService();
        private ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(ApplicationDbContext context, SignInManager<IdentityUser> signInManager) 
        {
            _context= context;
            _signInManager= signInManager;
        }

        public IList<Common.Models.ApiKey> apiKeys;

        public void OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            apiKeys = _context.ApiKey.Where(a => a.ApplicationUserID == userId).ToList();
        }

    }
}
