using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
public class JwtTokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private static readonly ConcurrentDictionary<string, string> RefreshTokens = new ConcurrentDictionary<string, string>();

    public JwtTokenService(string key, string issuer)
    {
        _key = key;
        _issuer = issuer;
    }

    public TokenResponse GenerateToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60), // Access token expires in 60 minutes
            signingCredentials: creds);

        var refreshToken = GenerateRefreshToken();

        RefreshTokens[username] = refreshToken;

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = refreshToken
        };
    }

    private string GenerateRefreshToken()
    {
        using var rng = new RNGCryptoServiceProvider();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public bool ValidateRefreshToken(string username, string refreshToken)
    {
        return RefreshTokens.TryGetValue(username, out var storedToken) && storedToken == refreshToken;
    }

    public void RevokeRefreshToken(string username)
    {
        RefreshTokens.TryRemove(username, out _);
    }
}