using System.Collections.Generic;
using AzurePizza01.Models;

namespace AzurePizza01.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}