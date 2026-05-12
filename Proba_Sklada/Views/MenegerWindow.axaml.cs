using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Inventori_Manager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Proba_Sklada;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }

    private void TextChanged(object o, TextChangedEventArgs e)
    {
    }

    private async void ExportPdf_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not MenegerViewModel vm)
                return;

            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить отчёт в PDF",
                SuggestedFileName = $"warehouse-report-{DateTime.Now:yyyyMMdd-HHmm}.pdf",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    new("PDF") { Patterns = new List<string> { "*.pdf" } }
                }
            });

            if (file is null)
                return;

            await vm.ExportReportPdfAsync(file.Path.LocalPath);

            await MessageBoxManager.GetMessageBoxStandard(
                    "Готово",
                    "Отчёт сохранён в PDF.",
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Success)
                .ShowAsync();
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка",
                    ex.Message,
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error)
                .ShowAsync();
        }
    }
}