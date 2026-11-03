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

            // Configuración dinámica según plataforma
#if ANDROID
            // 10.0.2.2 es el alias del localhost de tu PC para el emulador de Android
            string baseUrl = "https://10.0.2.2:7235/";
#else
            // Para Windows Machine
            string baseUrl = "https://localhost:7235/";
#endif

            // Configuración de red e inyección de HttpClient
            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHeaderHandler>();

                var httpHandler = new HttpClientHandler();

#if ANDROID && DEBUG
                // Permite omitir el error de certificado de desarrollo no confiable en Android
                httpHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

                handler.InnerHandler = httpHandler;
                return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
            });

            // Servicios de la aplicación
            builder.Services.AddScoped<IProductoService, ProductoService>();
            builder.Services.AddSingleton<ICarritoService, CarritoService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IVentaService, VentaService>();
            builder.Services.AddScoped<ISalidaService, SalidaService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();

            return builder.Build();
        }
    }
}