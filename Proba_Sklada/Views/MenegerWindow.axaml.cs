using Avalonia.Controls;
using Inventori_Manager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Proba_Sklada;

namespace Inventori_Manager;

public partial class MenegerWindow : Window
{
    public MenegerWindow()
    {
        InitializeComponent();
        DataContext = new MenegerViewModel(App.Services.GetRequiredService<Proba_Sklada.Hardik.Connector.dbBaza>());
    }

    private void SelectionChanged(object o, SelectionChangedEventArgs e)
    {
    }

    private void TextChanged(object o, TextChangedEventArgs e)
    {
    }
}