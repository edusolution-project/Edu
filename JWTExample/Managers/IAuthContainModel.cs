using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace JWTExample.Managers
{
    public interface IAuthContainModel
    {
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }
        Claim[] Claims { get; set; }
    }
}
