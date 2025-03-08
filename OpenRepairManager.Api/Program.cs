using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MariaDbServerVersion(new Version(11, 7, 2));

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
}
var firstRunSetting = SettingsService.GetSetting("firstrun");
if (firstRunSetting != null)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("DataSource=orm.db;Cache=shared",
        x => x.MigrationsAssembly("OpenRepairManager.SQLiteMigrations")));
    /*var connectionStringSettingtest = SettingsService.GetSetting("connectionstring");
    if (firstRunSetting.Value != "yes")
    {
        var connectionStringSetting = SettingsService.GetSetting("connectionstring");

        services.AddDbContext<ORMDbContext>(options =>
        {
            options.UseMySql(connectionStringSetting.Value, new MariaDbServerVersion(new Version(10, 4)),
                assembly => assembly.MigrationsAssembly(typeof(ORMDbContext).Assembly.FullName));
        });
        services.AddScoped<CheckKeyService>();*/
					
    }
    else
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, serverVersion,
                x => x.MigrationsAssembly("OpenRepairManager.MySQLMigrations")));
        
        /*SettingsService.AddOrUpdate(new()
        {
            Name = "connectionstring",
            Value = connectionString
        });
        services.AddDbContext<ORMDbContext>(options =>
        {
            options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4)),
                assembly => assembly.MigrationsAssembly(typeof(ORMDbContext).Assembly.FullName));
        });
        services.AddScoped<CheckKeyService>();*/
    }

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
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

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();