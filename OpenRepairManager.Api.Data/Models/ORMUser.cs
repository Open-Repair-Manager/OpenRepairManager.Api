using Microsoft.AspNetCore.Identity;

namespace OpenRepairManager.Api.Data.Models;

public class ORMUser : IdentityUser
{
    public string? Name { get; set; }
    
}