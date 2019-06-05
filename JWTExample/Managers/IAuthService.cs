using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace JWTExample.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }
        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
