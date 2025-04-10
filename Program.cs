using CartolaLigas.Services;
using CartolaLigas.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;


namespace CartolaLigas
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            // Registrar o novo HttpClient com uma URL configurada
            builder.Services.AddScoped<CustomHttpClientProvider>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<LigasService>();
            builder.Services.AddScoped<CartolaService>();
            builder.Services.AddScoped<TimeService>();
            builder.Services.AddAuthorizationCore();
            //builder.Services.AddOidcAuthentication(options =>
            //{
            //    // Configure your authentication provider options here.
            //    // For more information, see https://aka.ms/blazor-standalone-auth
            //    builder.Configuration.Bind("Local", options.ProviderOptions);
            //});

            await builder.Build().RunAsync();
        }
    }
}
