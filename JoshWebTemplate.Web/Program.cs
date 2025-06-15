using Blazored.LocalStorage;
using Blazored.Toast;
using JoshWebTemplate.Core.Services.Account.Api;
using JoshWebTemplate.Providers.Auth;
using JoshWebTemplate.Services.Account.Api;
using JoshWebTemplate.Web;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add core services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add authentication services FIRST
builder.Services.AddAuthentication()  // This registers IAuthenticationSchemeProvider
    .AddCookie();  // Or your preferred authentication scheme

// Add state management
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// Add UI services
builder.Services.AddMudServices();

// Configure Output Caching
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
});

// Add custom authentication provider
builder.Services.AddScoped<SupabaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SupabaseAuthStateProvider>());
builder.Services.AddScoped<ISupabaseAuthStateProvider>(sp =>
    sp.GetRequiredService<SupabaseAuthStateProvider>());
builder.Services.AddCascadingAuthenticationState();

// Add application services
builder.Services.AddScoped<IAccountApiService, AccountApiService>();

builder.Services.AddScoped<UserContext>();

// Configure Supabase
var url = builder.Configuration["SUPABASE:Url"] ??
    throw new InvalidOperationException("SUPABASE:Url not configured");
var key = builder.Configuration["SUPABASE:Key"] ??
    throw new InvalidOperationException("SUPABASE:Key not configured");

builder.Services.AddScoped(_ => new Supabase.Client(url, key, new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = false
}));

var app = builder.Build();

// Configure the HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication/Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();