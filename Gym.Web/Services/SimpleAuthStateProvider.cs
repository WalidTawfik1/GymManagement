using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Gym.Web.Services;

public class SimpleAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;

    public SimpleAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<DateTime>("auth_time");
            if (result.Success && result.Value > DateTime.Now.AddDays(-1))
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Admin") }, "SimpleAuth");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
        }
        catch { } // Handle pre-rendering where JS is not available

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public async Task LoginAsync()
    {
        await _localStorage.SetAsync("auth_time", DateTime.Now);
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Admin") }, "SimpleAuth");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task LogoutAsync()
    {
        await _localStorage.DeleteAsync("auth_time");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
    }
}
