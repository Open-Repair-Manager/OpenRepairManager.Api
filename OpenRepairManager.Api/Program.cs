using System.Net;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Api.Service;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Data.Models;
using OpenRepairManager.Api.Services;
using OpenRepairManager.Common.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ORMUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("ORMAdmin", "/");
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

bool firstrun = !File.Exists(DataProtectionService.ConfigFileFullPath);

if (firstrun)
{
    SettingsService.AddOrUpdate(new()
    {
        Name = "firstrun",
        Value = "yes"
    });
    SettingsService.AddOrUpdate(new()
    {
        Name="dbtype",
        Value = "sqlite"
    });
    SettingsService.AddOrUpdate(new()
    {
        Name = "connectionString",
        Value = "DataSource=orm.db;Cache=shared"
    });
    var connectionStringSetting = SettingsService.GetSetting("connectionString");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionStringSetting.Value,
            x => x.MigrationsAssembly("OpenRepairManager.SQLiteMigrations")));
    
}
else
{
    var dboption = SettingsService.GetSetting("dbtype");
    if (dboption.Value == "mysql")
    {
        try
        {
            var connectionStringSetting = SettingsService.GetSetting("connectionString");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionStringSetting.Value, new MariaDbServerVersion(new Version(11,3,0)),
                    x => x.MigrationsAssembly("OpenRepairManager.MySQLMigrations")));
        }
        catch (Exception Ex)
        {
            SettingsService.AddOrUpdate(new Setting()
            {
                Name = "dberror",
                Value = "true"
            });
        }
    }
    else
    {
        var connectionStringSetting = SettingsService.GetSetting("connectionString");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionStringSetting.Value,
                x => x.MigrationsAssembly("OpenRepairManager.SQLiteMigrations")));
    }
}

builder.Services.AddScoped<CheckKeyService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
        SettingsService.AddOrUpdate(new Setting()
        {
            Name = "dberror",
            Value = "false"
        });
    }
    catch (Exception Ex)
    {
        SettingsService.AddOrUpdate(new Setting()
        {
            Name = "dberror",
            Value = "true"
        });
    }
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

Console.WriteLine("Starting Open Repair Manager");

Console.WriteLine(@"  ____                     _____                  _        __  __                                   
 / __ \                   |  __ \                (_)      |  \/  |                                  
| |  | |_ __   ___ _ __   | |__) |___ _ __   __ _ _ _ __  | \  / | __ _ _ __   __ _  __ _  ___ _ __ 
| |  | | '_ \ / _ \ '_ \  |  _  // _ \ '_ \ / _` | | '__| | |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '__|
| |__| | |_) |  __/ | | | | | \ \  __/ |_) | (_| | | |    | |  | | (_| | | | | (_| | (_| |  __/ |   
 \____/| .__/ \___|_| |_| |_|  \_\___| .__/ \__,_|_|_|    |_|  |_|\__,_|_| |_|\__,_|\__, |\___|_|   
       | |                           | |                                             __/ |          
       |_|                           |_|                                            |___/           
");

Console.WriteLine("Server API address is");
var host = Dns.GetHostEntry(Dns.GetHostName());
foreach (var ip in host.AddressList)
{
    if (ip.AddressFamily == AddressFamily.InterNetwork)
    {
        Console.WriteLine(ip.ToString() + "/api/");
        //app.Urls.Add("http://" + ip.ToString() + ":5000");
    }
}


app.Run();