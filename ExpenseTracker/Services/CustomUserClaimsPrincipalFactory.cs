using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ExpenseTracker.Services;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (!string.IsNullOrEmpty(user.DisplayName))
        {
            identity.AddClaim(new Claim("DisplayName", user.DisplayName));
        }
        identity.AddClaim(new Claim("BaseCurrency", user.BaseCurrency));
        var symbol = user.BaseCurrency switch
        {
            "EUR" => "€",
            "GBP" => "£",
            "INR" => "₹",
            "JPY" => "¥",
            "CAD" => "CA$",
            "AUD" => "A$",
            _ => "$"
        };
        identity.AddClaim(new Claim("CurrencySymbol", symbol));
        return identity;
    }
}
