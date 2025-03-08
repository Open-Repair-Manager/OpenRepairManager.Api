using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Common.Models;

namespace OpenRepairManager.Api.Data;

public class SQLiteContext : IdentityDbContext
{
    public DbSet<Session> Session { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<RepairItem> RepairItem { get; set; }
    public DbSet<ApiKey> ApiKey { get; set; }
    public SQLiteContext(DbContextOptions<SQLiteContext> options)
        : base(options)
    {
    }
}