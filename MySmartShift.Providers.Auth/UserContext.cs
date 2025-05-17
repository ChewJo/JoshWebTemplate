using Microsoft.AspNetCore.Components.Authorization;
using MySmartShift.Core.Models.Account;
using MySmartShift.Core.Services.Account.Api;

namespace MySmartShift.Providers.Auth;

public class UserContext
{
    public UserModel? CurrentUser { get; private set; }
    private readonly IAccountApiService _accountApi;
    private readonly AuthenticationStateProvider _authProvider;
    private bool _hasFetchedUser = false;

    public UserContext(IAccountApiService accountApi, AuthenticationStateProvider authProvider)
    {
        _accountApi = accountApi;
        _authProvider = authProvider;

        _authProvider.AuthenticationStateChanged += async (task) =>
        {
            var authState = await task;

            if (authState.User.Identity?.IsAuthenticated == true)
            {
                await EnsureUserAsync();
            }
            else
            {
                Reset();
            }
        };
    }

    public async Task EnsureUserAsync()
    {
        if (_hasFetchedUser) return;

        var authState = await _authProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            CurrentUser = await _accountApi.GetCurrentUserModelAsync();
            _hasFetchedUser = true;
        }
    }

    public void Reset()
    {
        _hasFetchedUser = false;
        CurrentUser = null;
    }
}
