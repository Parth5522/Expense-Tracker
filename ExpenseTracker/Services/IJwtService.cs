using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
