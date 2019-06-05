using JWTExample.Managers;
using JWTExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JWTExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IAuthContainModel model = GetJWTContainerModel("Moshe Binieli", "mmoshikoo@gmail.com");
            IAuthService authService = new AuthService(model.SecretKey);

            string token = authService.GenerateToken(model);

            if (!authService.IsTokenValid(token))
                throw new UnauthorizedAccessException();
            else
            {
                List<Claim> claims = authService.GetTokenClaims(token).ToList();

                Console.WriteLine(claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.Name)).Value);
                Console.WriteLine(claims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.Email)).Value);
            }
            Console.ReadKey();
        }

        #region Private Methods
        private static IAuthContainModel GetJWTContainerModel(string name, string email)
        {
            return new AuthContainModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }
        #endregion
    }
}
