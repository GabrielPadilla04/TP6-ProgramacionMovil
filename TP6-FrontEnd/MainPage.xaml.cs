namespace TP6_FrontEnd
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            blazorWebView.BlazorWebViewInitializing += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(">>> WEBVIEW: Iniciando...");
            };

            blazorWebView.BlazorWebViewInitialized += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(">>> WEBVIEW: Inicializado correctamente.");
            };
        }
    }
}