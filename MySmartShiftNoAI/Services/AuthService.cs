using Blazored.LocalStorage;
using FluentResults;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MySmartShiftNoAI.Auth;
using MySmartShiftNoAI.Models;
using MySmartShiftNoAI.Services.Interfaces;
using Supabase.Gotrue;
using System.Text.Json;

namespace MySmartShiftNoAI.Services;

public class AuthService : IAuthService
{
    private const string SESSION_KEY = "supabase_session";
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IConfiguration _configuration;
    private readonly Supabase.Client _supabase;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly Supabase.Client _serviceClient;

    public AuthService(
        ILocalStorageService localStorage,
        NavigationManager navigation,
        IConfiguration configuration,
        Supabase.Client supabase,
        AuthenticationStateProvider authStateProvider)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _configuration = configuration;
        _supabase = supabase;
        _authStateProvider = authStateProvider;

        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:ServiceKey"];
        
        Console.WriteLine($"Initializing service client with URL: {url}");
        Console.WriteLine($"Service key starts with: {key?.Substring(0, 10)}...");

        _serviceClient = new Supabase.Client(
            url!,
            key!,
            new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            });
    }

    public async Task<Result> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _supabase.Auth.SignIn(email, password);
            if (session?.AccessToken == null)
            {
                Console.WriteLine($"Login failed for user {email}");
                return Result.Fail("Login failed");
            }

            Console.WriteLine($"Login successful for user {session.User.Id}");

            // Store the session in localStorage
            var json = JsonSerializer.Serialize(session);
            await _localStorage.SetItemAsStringAsync(SESSION_KEY, json);

            ((SupabaseAuthenticationStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> RegisterAsync(LoginArgs model)
    {
        try
        {
            Console.WriteLine($"Attempting to register user {model.Username}");
            
            // Register the user in Supabase Auth
            var response = await _supabase.Auth.SignUp(model.Username, model.Password);
            if (response?.User == null)
            {
                Console.WriteLine("Registration failed - no user returned");
                return Result.Fail("Registration failed");
            }

            Console.WriteLine($"Registration successful for user {response.User.Id}");
            // Debug: Print the entire response.User object as JSON
            try
            {
                Console.WriteLine($"response.User: {JsonSerializer.Serialize(response.User)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to serialize response.User: {ex.Message}");
            }
            // Debug: Print the value of response.User.Id
            Console.WriteLine($"response.User.Id: {response.User.Id}");

            try
            {
                Console.WriteLine("Creating new user record...");
                var dbUser = new DbUser
                {
                    Id = Guid.Parse(response.User.Id),
                    Email = model.Username,
                    Role = "user",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var dbResponse = await _serviceClient.From<DbUser>().Insert(dbUser);
                var statusCode = dbResponse.ResponseMessage?.StatusCode;
                Console.WriteLine($"User creation HTTP status: {statusCode}");

                if (!dbResponse.Models.Any())
                {
                    Console.WriteLine("Failed to create user record in database");
                    return Result.Fail($"Failed to create user record. Status: {statusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user record: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Result.Fail($"Registration succeeded but failed to create user record: {ex.Message}");
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> LogoutAsync()
    {
        try
        {
            await _supabase.Auth.SignOut();
            await _localStorage.RemoveItemAsync(SESSION_KEY);
            ((SupabaseAuthenticationStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
            _navigation.NavigateTo("/account/login");
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return Result.Fail(ex.Message);
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var json = await _localStorage.GetItemAsStringAsync(SESSION_KEY);
            if (string.IsNullOrEmpty(json))
                return false;

            var session = JsonSerializer.Deserialize<Session>(json);
            return session?.AccessToken != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IsAuthenticated error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<Result> RefreshTokenAsync()
    {
        try
        {
            var session = await _supabase.Auth.RetrieveSessionAsync();
            if (session == null)
            {
                Console.WriteLine("No active session");
                return Result.Fail("No active session");
            }

            // Update stored session
            var json = JsonSerializer.Serialize(session);
            await _localStorage.SetItemAsStringAsync(SESSION_KEY, json);

            ((SupabaseAuthenticationStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RefreshToken error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return Result.Fail(ex.Message);
        }
    }

    // Add this method to restore the Supabase session from local storage on app startup
    public async Task RestoreSessionAsync()
    {
        try
        {
            var json = await _localStorage.GetItemAsStringAsync(SESSION_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                var session = JsonSerializer.Deserialize<Session>(json);
                if (session != null && session.AccessToken != null)
                {
                    Console.WriteLine($"[RestoreSessionAsync] Restoring session for user: {session.User?.Id}");
                    await _supabase.Auth.SetSession(session.AccessToken, session.RefreshToken);
                }
                else
                {
                    Console.WriteLine("[RestoreSessionAsync] No valid session found in local storage.");
                }
            }
            else
            {
                Console.WriteLine("[RestoreSessionAsync] No session found in local storage.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RestoreSessionAsync] Error: {ex.Message}");
        }
    }
}