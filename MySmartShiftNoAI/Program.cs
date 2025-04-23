using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MySmartShiftNoAI;
using MySmartShiftNoAI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddTransient<AuthService>();


// Supabase
var url = builder.Configuration["SUPABASE:Url"];
var key = builder.Configuration["SUPABASE:Key"];

var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};

builder.Services.AddScoped(provider => new Supabase.Client(url, key, options));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

await builder.Build().RunAsync();
