using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using TP6_FrontEnd.Services;

namespace TP6_FrontEnd
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Registramos el handler para que intercepte y agregue el Bearer Token
            builder.Services.AddTransient<AuthHeaderHandler>();

            // Configuración de red e inyección de HttpClient
            string baseUrl = "https://h1tjpbkm-7235.brs.devtunnels.ms/";
            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHeaderHandler>();
                handler.InnerHandler = new HttpClientHandler();
                return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
            });

            // Servicios de la aplicación
            builder.Services.AddScoped<IProductoService, ProductoService>();
            builder.Services.AddSingleton<ICarritoService, CarritoService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IVentaService, VentaService>(); 

            return builder.Build();
        }
    }
}