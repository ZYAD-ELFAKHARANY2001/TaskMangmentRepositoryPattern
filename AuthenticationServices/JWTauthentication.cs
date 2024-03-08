using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Mangment_Api.Helpers;
using Task_Mangment_Api.Models;

namespace Task_Mangment_Api.AuthenticationServices
{
    public class JWTauthentication
    {
        private readonly UserManager<User> _UserManager;
        private readonly JWT _jwt;
        public JWTauthentication(UserManager<User> UserManager, JWT jwt)
        {
            this._UserManager = UserManager;
            _jwt = jwt;

        }
        public async Task<JwtSecurityToken> CreateJwtToken(User user, bool rememberme = false)
        {
            var userClaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),new Claim("uid", user.Id),
            new Claim("username",user.UserName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
             issuer: _jwt.ValidIssuer,
             audience: _jwt.ValidAudience,
             claims: claims,
             expires: (rememberme == true) ? DateTime.UtcNow.AddDays(_jwt.Duration) : DateTime.UtcNow.AddDays(30),
             signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
