using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Web.BaseSecurity
{
    public class SecurityManager
    {
        public async void SignIn(HttpContext httpContext, string username, List<string> roles)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(getUserClaims(username, roles), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        public async void SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        private IEnumerable<Claim> getUserClaims(string username, List<string> roles)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, username));
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            return claims;
        }
    }
}
