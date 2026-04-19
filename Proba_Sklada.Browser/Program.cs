using Avalonia;
using Avalonia.Browser;
using Proba_Sklada;

internal sealed partial class Program
{
    private static void Main(string[] args) =>
        BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out")
            .GetAwaiter()
            .GetResult();

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}