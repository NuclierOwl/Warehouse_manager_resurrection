using Avalonia.Controls;

namespace Inventori_Manager.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        //Get();
    }
    /*
    private void SelectionChanged(object o, SelectionChangedEventArgs e)
    {
        Get();
    }

    private void TextChanged(object o, TextChangedEventArgs e)
    {
        Get();
    }

    private void Get()
    {
        using (var db = new dbBaza())
        {
            List<inventory> AllItem = db.inventories.ToList();


            if (SerchBox != null && !string.IsNullOrEmpty(SerchBox.Text))
            {
                AllItem = AllItem.Where(h => h.product.name.Contains(SerchBox.Text)).ToList();
            }

            switch (ComboFilter.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    AllItem = AllItem.OrderByDescending(x => x.product.name).ToList();
                    break;
                case 2:
                    AllItem = AllItem.OrderBy(x => x.product.name).ToList();
                    break;
                default:
                    break;
            }

            List<inventory> EstItem = AllItem
                .Where(c => c.kolichestvo > 0)
                .ToList();

            List<inventory> NetItem = AllItem
                .Where(c => c.kolichestvo == 0)
                .ToList();

            AllBox.ItemsSource = AllItem;
            EstBox.ItemsSource = EstItem;
            NetBox.ItemsSource = NetItem;

        }
    }

    private void LF_Kolichestvo(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox Kol_vo && Kol_vo.DataContext is inventory item)
        {
            if (string.IsNullOrWhiteSpace(Kol_vo.Text))
            {
                item.kolichestvo = 0;
                SaveItemKolichestvo(item);
                return;
            }

            if (int.TryParse(Kol_vo.Text, out int newQuantity))
            {
                item.kolichestvo = newQuantity;
                SaveItemKolichestvo(item);
            }
            else
            {
                Kol_vo.Text = item.kolichestvo.ToString() ?? "0";
            }
        }
    }

    private async void SaveItemKolichestvo(inventory item)
    {
        using (var db = new dbBaza())
        {
            var exo = db.inventories.FirstOrDefault(h => h.id == item.id);
            if (exo != null)
            {
                exo.kolichestvo = item.kolichestvo;
                await db.SaveChangesAsync();
                Get();
            }

        }
    }

    private async void Add_New_Click(object o, RoutedEventArgs e)
    {
        var addWindow = new AddItemWindow();
        var result = await addWindow.ShowDialog<bool>(this);
        if (result)
        {
            using (var db = new dbBaza())
            {
                var loc = db.inventories.FirstOrDefault(i => i.location.rack.name == addWindow.Location);
                var prod = db.inventories.FirstOrDefault(i => i.product.name == addWindow.ItemName);
                var nowinka = db.inventories.FirstOrDefault(i => i.product.name == addWindow.ItemName && i.location.rack.name == addWindow.Location);
                if (nowinka != null && loc == null)
                {
                    return;
                }

                var pribavka = new inventory()
                {
                    product_id = prod.id,
                    kolichestvo = addWindow.Quantity,
                    location_id = loc.id,
                    last_updated = DateAndTime.Now
                };
                Get();
            }
        }
    }


    private void DellFromDB(object o, RoutedEventArgs e)
    {
        if (AllBox.SelectedItem is inventory sect)
        {
            using (var db = new dbBaza())
            {
                var deli = db.inventories.Where(i => i.product.name == sect.product.name && i.id == sect.id).ExecuteDelete();
                db.SaveChanges();
                Get();
            }
        }
    } */
}
