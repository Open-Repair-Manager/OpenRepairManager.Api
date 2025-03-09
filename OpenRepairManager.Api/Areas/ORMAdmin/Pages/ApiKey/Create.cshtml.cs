using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using OpenRepairManager.Api.Services;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Service;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.ApiKey
{
    public class CreateModel : PageModel
    {
        private ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private IHttpContextAccessor _contextAccessor;

        public CreateModel(ApplicationDbContext context, SignInManager<IdentityUser> signInManager, IHttpContextAccessor accessor)
        {
            _context = context;
            _signInManager = signInManager;
            _contextAccessor = accessor;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnPost() 
        {
            IApiKeyService service= new ApiKeyService();
            string key = service.GenerateApiKey();
            Common.Models.ApiKey Key = new Common.Models.ApiKey()
            {
                Key = key,
                ApplicationUserID = _signInManager.UserManager.GetUserId(_contextAccessor.HttpContext.User)
            };
            try
            {
                _context.Add(Key);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            catch(Exception ex) 
            {

            }
            return RedirectToPage("Index");
        }
    }
}
