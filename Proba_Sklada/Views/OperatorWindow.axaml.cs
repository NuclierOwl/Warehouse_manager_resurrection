using Avalonia.Controls;
using Avalonia.Interactivity;
using Inventori_Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using Proba_Sklada;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            IntroBox.Text = $"Оператор — {us.full_name}";
            _user = us;
            DataContext = new InventoryViewModel(App.Services.GetRequiredService<dbBaza>(), us);
            Get();
        }

        private async void Button_Click(object e, RoutedEventArgs er)
        {
            Get();
        }

        private void Get()
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            var inv = db.inventories.Include(e => e.location).Include(e => e.product).ToList().AsEnumerable(); ;

            #region Фильтры 
            if (ProductsBox.SelectedIndex > -1)
                inv = inv.Where(i => i.product.name == ProductsBox.SelectedValue.ToString());

            if (LocationBox.SelectedIndex > -1)
                inv = inv.Where(i => i.location.location_code == LocationBox.SelectedValue.ToString());

            if (!string.IsNullOrWhiteSpace(BatchBox.Text))
            {
                string filter = BatchBox.Text.Trim();
                inv = inv.Where(i =>
                    (i.batch_number != null && i.batch_number.Contains(filter) ||
                    (i.serial_number != null && i.serial_number.Contains(filter))));
            }

            if (PositiveBox.IsChecked == true)
                inv = inv.Where(i => i.kolichestvo > 0);
            #endregion

            InventoryBox.ItemsSource = inv;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private void Obnovka_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Get();
        }

        private void Obnovka_TextChanged(object sender, TextChangedEventArgs e)
        {
            Get();
        }

        private void Obnovka_CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            Get();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ProductsBox.SelectedIndex = -1;
            LocationBox.SelectedIndex = -1;
            BatchBox.Text = string.Empty;
            PositiveBox.IsChecked = true;
            Get();
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

        private async void SelectionProd(object? sender, SelectionChangedEventArgs e)
        {
            try
            {


                var db = App.Services.GetRequiredService<dbBaza>();
                var nuz = (sender as ComboBox);
                if (nuz?.SelectedValue != null)
                {
                    string tovar = nuz.SelectedItem.ToString();
                    if (!string.IsNullOrEmpty(tovar))
                    {
                        var product = db.products.AsNoTracking().FirstOrDefault(p => p.name == tovar);
                        if (product != null)
                        {
                            CenikBox.Value = (decimal)product.selling_price;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Внимание", ex.ToString(), MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
            }
        }

        private void ResetFilter()
        {
            ProductsBox.SelectedValue = null;
            LocationBox = null;
            BatchBox.Text = string.Empty;
            PositiveBox.IsChecked = true;
            Get();
        }
    }
}