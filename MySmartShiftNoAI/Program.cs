using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MySmartShiftNoAI;
using MySmartShiftNoAI.Auth;
using MySmartShiftNoAI.Services;
using MySmartShiftNoAI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

// Register services
builder.Services.AddScoped<AuthenticationStateProvider, SupabaseAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add auth
builder.Services.AddAuthorizationCore();

// Supabase
var url = builder.Configuration["SUPABASE:Url"];
var key = builder.Configuration["SUPABASE:Key"];

var options = new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = false // Disable realtime websocket connection
};

builder.Services.AddScoped(provider => new Supabase.Client(url, key, options));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();

// Initialize Supabase
var supabase = app.Services.GetRequiredService<Supabase.Client>();
await supabase.InitializeAsync();

// --- Restore Supabase session on app startup ---
var authService = app.Services.GetRequiredService<IAuthService>();
if (authService is AuthService concreteAuthService)
{
    await concreteAuthService.RestoreSessionAsync();
}
// --- End session restore ---

await app.RunAsync();
