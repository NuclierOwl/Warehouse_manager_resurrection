using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using Proba_Sklada;
using Proba_Sklada.Hardik;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventori_Manager;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void KnopkaTest_Click(object? sender, RoutedEventArgs e)
    {
        UserName.Text = "sklad 1283";
        UserPass.Text = "123456";
    }

    private async void KnopkaWhoda_Click(object sender, RoutedEventArgs e)
    {

        string login = UserName.Text;
        string Password = UserPass.Text;

        try
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            List<user> users = db.users.ToList();
            var userok = users.FirstOrDefault(u => u.username == login && u.password == Password);
            var next = new Window();

            if (userok != null)
            {
                if (userok.is_active == true)
                {
                    switch (userok.role.ToLower())
                    {
                        case ("operator"):
                            next = new OperatorWindow(userok);
                            next.Show();
                            this.Close();
                            break;
                        case ("admin"):
                            next = new AdminWindow();
                            next.Show();
                            this.Close();
                            break;
                        case ("manager"):
                            next = new MenegerWindow();
                            next.Show();
                            this.Close();
                            break;
                    }
                }
                else
                {
                    //var mess =
                }
            }
            else
            {
                Dop.WarningWindow("¬веди логин и пароль", this).Show();
            }
        }
        catch (System.Exception ex)
        {
            try
            {
                var mes = await MessageBoxManager.GetMessageBoxStandard("¬нимание", ex.ToString(), MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Warning).ShowAsync();
            }
            catch (System.Exception exc)
            {
                await WarningWindow(exc.ToString() + ex.ToString(), this);
            }
        }
    }

    public async Task WarningWindow(string messeg, Window win)
    {
        var mes = new Window
        {
            Width = 300,
            Height = 400,
            Title = "Achtung"
        };

        var ponel = new Grid();

        var text = new TextBlock
        {
            Text = messeg,
            Margin = new Thickness(20, 20, 20, 10)
        };

        var but = new Button
        {
            Content = "OK",
            Width = 80,
            Margin = new Thickness(0, 0, 20, 20)
        };

        but.Click += (_, _) => mes.Close();

        ponel.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        ponel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        ponel.Children.Add(text);
        Grid.SetRow(text, 0);

        ponel.Children.Add(but);
        Grid.SetRow(but, 1);

        mes.Content = ponel;

        await mes.ShowDialog(win);
    }
}