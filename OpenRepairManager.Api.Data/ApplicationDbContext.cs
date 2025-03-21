using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Data.Models;
using OpenRepairManager.Common.Models;

namespace OpenRepairManager.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ORMUser>
{
    public DbSet<Session> Session { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<RepairItem> RepairItem { get; set; }
    public DbSet<ApiKey> ApiKey { get; set; }
    
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}