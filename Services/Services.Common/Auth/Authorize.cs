using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace Auth;

public static class Authorize
{

    private static string accessTokenKey = "this is an incredibly secure key";
    private static string refreshTokenKey = "this is an incredibly strong secure key";

    public static string? GetUserEmail(string? bearerToken)
    {
        var uid = GetClaim(JwtRegisteredClaimNames.Email, bearerToken);

        return uid;
    }
    public static string? GetUserId(string? bearerToken)
    {
        var uid = GetClaim(JwtRegisteredClaimNames.Sub, bearerToken);

        return uid;
    }

    public static string? GetClaim(string claim, string bearerToken)
    {

        try
        {
            var claims = GetTokenClaim(bearerToken);

            if (claim == ClaimTypes.Role)
            {
                List<string> roles = new List<string>();

                foreach (var clm in claims)
                {
                    if (clm.Type == claim)
                        roles.Add(clm.Value);
                }

                var joinedRoles = string.Join(",", roles.ToArray());

                return joinedRoles;
            }

            var filteredClaim = claims.First(c => c.Type == claim).Value;

            return filteredClaim;
        }
        catch (SecurityTokenExpiredException _)
        {
            Console.WriteLine("Token Expired");
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine("Invalid Token");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }

        return null;
    }

    // Verify and Decode Token
    public static IEnumerable<Claim> GetTokenClaim(string bearerToken)
    {   
        if (VerifyToken(bearerToken) == null)
            return null;

        var accessToken = bearerToken.Split(" ")[1];

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);

        return token.Claims;
    }

    private static Claim[] GenerateClaims(string id, string email, string[] roles)
    {
        Claim[] claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, roles != null ? JsonSerializer.Serialize(roles) : string.Empty,JsonClaimValueTypes.JsonArray)
        };

        return claims;
    }

    public static string GenerateToken(string id, string email, string[] roles)
    {
        int expireDay = 15 * 60 * 60;

        Claim[] claims = GenerateClaims(id, email, roles);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKey));

        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(id.ToString(), null, claims, null, DateTime.Now.AddSeconds(expireDay)); // 7 seconds
                                                                                                             // var payload = new JwtPayload(id.ToString(), null, claims, null, DateTime.Today.AddDays(expireDay)); // 7 seconds
        var securityToken = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);

    }
    public static string GenerateRefreshToken(string id, string email, string[] roles)
    {
        int expireDay = 60 * 60 * 24 * 2;

        Claim[] claims = GenerateClaims(id, email, roles);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenKey));

        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(id.ToString(), null, claims, null, DateTime.Now.AddSeconds(expireDay)); // 7 seconds

        var securityToken = new JwtSecurityToken(header, payload);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);

    }

    public static JwtSecurityToken VerifyToken(string bearerToken)
    {
        var accessToken = bearerToken.Split(" ")[1];
        Console.WriteLine(accessToken);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(accessTokenKey);

        tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false,
        }, out SecurityToken validatedToken);

        return (JwtSecurityToken)validatedToken;
    }

    public static JwtSecurityToken VerifyRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(refreshTokenKey);

        tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false,
        }, out SecurityToken validatedToken);

        return (JwtSecurityToken)validatedToken;
    }

}