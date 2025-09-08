using Microsoft.AspNetCore.Identity;

namespace FitBridge_Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}