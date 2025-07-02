using Microsoft.AspNetCore.DataProtection;

namespace Jollicow.Services;

public class TokenService
{
    private readonly IDataProtector _dataProtector;

    public TokenService(IDataProtectionProvider provider)
    {
        _dataProtector = provider.CreateProtector("secure-token");
    }

    public string GenerateToken(string tableId, string restaurantId)
    {
        var raw = $"{tableId}|{restaurantId}|{DateTime.UtcNow.AddHours(7):yyyy-MM-dd HH:mm:ss}";
        return _dataProtector.Protect(raw);
    }

    public (string tableId, string restaurantId)? TryDecrypt(string token, int validMinutes = 30)
    {
        try
        {
            var decrypted = _dataProtector.Unprotect(token);
            var parts = decrypted.Split('|');
            if (parts.Length != 3) return null;

            var tableId = parts[0];
            var restaurantId = parts[1];
            var time = DateTime.ParseExact(parts[2], "yyyy-MM-dd HH:mm:ss", null);
            if ((DateTime.UtcNow.AddHours(7) - time).TotalMinutes > validMinutes) return null;

            return (tableId, restaurantId);
        }
        catch
        {
            return null;
        }
    }
}
