using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Api.Service;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Services;
using OpenRepairManager.Common.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

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

app.Run();