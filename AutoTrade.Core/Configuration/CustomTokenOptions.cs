namespace AutoTrade.Core.Configuration;

public class CustomTokenOptions
{
    public List<string> Audience { get; set; } = new();
    public string Issuer { get; set; } = string.Empty;
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
    public string SecurityKey { get; set; } = string.Empty;
}