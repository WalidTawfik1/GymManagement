using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Gym.Web.Services;

public class SecurityService
{
    private readonly string _filePath;

    public SecurityService(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "security.json");
    }

    public async Task<bool> VerifyPasswordAsync(string password)
    {
        var hash = await GetStoredHashAsync();
        return HashPassword(password) == hash;
    }

    public async Task ChangePasswordAsync(string newPassword)
    {
        var hash = HashPassword(newPassword);
        await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(new SecurityData { PasswordHash = hash }));
    }

    private async Task<string> GetStoredHashAsync()
    {
        if (!File.Exists(_filePath))
        {
            // Default password is "admin"
            var defaultHash = HashPassword("admin");
            await ChangePasswordAsync("admin");
            return defaultHash;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var data = JsonSerializer.Deserialize<SecurityData>(json);
            return data?.PasswordHash ?? HashPassword("admin");
        }
        catch
        {
            return HashPassword("admin");
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
        return Convert.ToBase64String(bytes);
    }

    private class SecurityData
    {
        public string PasswordHash { get; set; } = "";
    }
}
