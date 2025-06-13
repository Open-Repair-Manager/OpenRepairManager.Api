using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Services;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace OpenRepairManager.Api.Areas.ORMAdmin.Pages.Setup
{
    [AllowAnonymous]
    [BindProperties]
    public class DatabaseSetupModel : PageModel
    {
        //MySQL variables
        public bool useMySQL { get; set; }
        public string? serverName { get; set; }
        public uint serverPort { get; set; }
        public string? databaseName { get; set; }
        public string? userPassword { get; set; }
        public string? userName { get; set; }
        
        public bool DBError { get; set; }

        private readonly ApplicationDbContext _context;
        private IApplicationLifetime _applicationLifetime { get; set; }
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSetupModel(ApplicationDbContext context, IApplicationLifetime applicationLifetime, RoleManager<IdentityRole> roleManager)
        {
            if (SettingsService.GetSetting("dberror").Value == "true")
            {
                DBError = true;
                
            }
            _context = context;
            _applicationLifetime = applicationLifetime;
            _roleManager = roleManager;
        }

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
            var dbchange = SettingsService.GetSetting("dbchanged");
            if (dbchange != null)
            {
                
                if(dbchange.Value == "yes")
                {
                    if (DBError)
                    {
                        return Page();
                    }
                    _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                    return RedirectToPage("AccountSetup");
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (useMySQL)
            {
                var connectionstring = new MySqlConnectionStringBuilder()
                {
                    Server = serverName,
                    Port = serverPort,
                    Database = databaseName,
                    UserID = userName,
                    Password = userPassword
                }.ToString();
                _context.Database.SetConnectionString(connectionstring);
                SettingsService.AddOrUpdate(new()
                {
                    Name="dbtype",
                    Value = "mysql"
                });
                SettingsService.AddOrUpdate(new()
                {
                    Name = "connectionstring",
                    Value = connectionstring
                });
                SettingsService.AddOrUpdate(new()
                {
                    Name = "dbchanged",
                    Value = "yes"
                });
                //await _context.Database.MigrateAsync();
                _applicationLifetime.StopApplication();
                return new ContentResult()
                {
                    Content = "<head></head><p>Database Setting Saved! If you are running this app on a web server, please <a href=\"/ORMAdmin/Setup/DatabaseSetup\">Click Here</a> to continue after 30 seconds. If you are running this from the executable directly, please restart the app before clicking the link. If the connection fails, the form will be redisplayed</p>",
                    ContentType = "text/html",
                    StatusCode = 200
                };
            }
            else
            {
                SettingsService.AddOrUpdate(new()
                {
                    Name="dbtype",
                    Value = "sqlite"
                });
                SettingsService.AddOrUpdate(new()
                {
                    Name = "dbchanged",
                    Value = "yes"
                });
                SettingsService.AddOrUpdate(new()
                {
                    Name = "connectionString",
                    Value = "DataSource=orm.db;Cache=shared"
                });
                //await _context.Database.MigrateAsync();
                _applicationLifetime.StopApplication();
                return new ContentResult()
                {
                    Content = "<head></head><p>Database Setting Saved! If you are running this app on a web server, please <a href=\"/ORMAdmin/Setup/DatabaseSetup\">Click Here</a> to continue. If you are running this from the executable directly, please restart the app before clicking the link. If the connection fails, the form will be redisplayed</p>",
                    ContentType = "text/html",
                    StatusCode = 200
                };
            }
            return Page();
        }
        
        
    }
}
