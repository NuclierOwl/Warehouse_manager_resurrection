using Avalonia.Controls;
using Avalonia.Interactivity;
using Inventori_Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proba_Sklada;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventori_Manager
{
    public partial class OperatorWindow : Window
    {
        user _user;
        public OperatorWindow()
        {
            InitializeComponent();
            //FioBox.Text = "Гость";
            DataContext = new InventoryViewModel(App.Services.GetRequiredService<dbBaza>());
            Get();
        }
        public OperatorWindow(user us)
        {
            InitializeComponent();
            //FioBox.Text = $"{us.username}";
            _user = us;
            DataContext = new InventoryViewModel(App.Services.GetRequiredService<dbBaza>(), us);
            Get();
        }

        private void Get()
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            List<inventory> inv = db.inventories.Include(e => e.location).Include(e => e.product).ToList();

            InventoryBox.ItemsSource = inv;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private void Cheaker()
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            List<inventory> inv = db.inventories.Include(e => e.location).Include(e => e.product).ToList();

            foreach (var d in inv)
            {
                if (d.last_updated < DateTime.Now.AddMonths(-1) && d.kolichestvo == 0)
                {
                    db.inventories.Remove(d);
                }
            }
        }
    }
}