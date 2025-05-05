using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MySmartShift.Core.Services.Account.Api;
using MySmartShift.Providers.Auth;
using MySmartShift.Services.Account.Api;
using MySmartShiftNoAI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Possibly remove in future, not currently using.
builder.Services.AddBlazoredLocalStorage();

// Register application services
builder.Services.AddScoped<IAccountApiService, AccountApiService>();

// Register authentication state provider
builder.Services.AddScoped<SupabaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SupabaseAuthStateProvider>());
builder.Services.AddAuthorizationCore();


// Supabase setup
var url = builder.Configuration["SUPABASE:Url"];
var key = builder.Configuration["SUPABASE:Key"];
var options = new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = false
};

builder.Services.AddScoped(_ => new Supabase.Client(url!, key, options));

// HttpClient for API Access
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});


var app = builder.Build();

// Initialise Supabase
var supabase = app.Services.GetRequiredService<Supabase.Client>();
await supabase.InitializeAsync();

await app.RunAsync();
