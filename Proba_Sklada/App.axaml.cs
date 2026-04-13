using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System;
using System.Linq;
using Avalonia.Markup.Xaml;
using Proba_Sklada.ViewModels;
using Inventori_Manager;
using Inventori_Manager.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Proba_Sklada.Hardik.Connector;

namespace Proba_Sklada;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = BuildServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new LoginWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(System.AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);

        // Desktop app: simpler to use transient DbContext (no scopes needed)
        services.AddDbContext<dbBaza>(
            opt => opt.UseNpgsql(configuration.GetConnectionString("dbBaza")),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

        // ViewModels
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}