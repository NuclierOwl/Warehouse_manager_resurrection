using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Proba_Sklada;

namespace Inventori_Manager;

public partial class AdminWindow : Window
{
    //     
    private Dictionary<int, user> modifiedUsers = new Dictionary<int, user>();

    private readonly ObservableCollection<product_category> _categories = new();
    private readonly ObservableCollection<product> _products = new();
    private readonly ObservableCollection<unit> _units = new();

    public AdminWindow()
    {
        InitializeComponent();
        Loaded += AdminWindow_Loaded;
    }

    private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Get();
        LoadUsersForEditing();
        LoadUnits();
    }

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
        var db = App.Services.GetRequiredService<dbBaza>();
        List<user> allUsers = db.users.ToList();

        if (SerchBox != null && !string.IsNullOrEmpty(SerchBox.Text))
        {
            allUsers = allUsers.Where(u =>
                u.full_name.Contains(SerchBox.Text) ||
                u.username.Contains(SerchBox.Text) ||
                u.email.Contains(SerchBox.Text)).ToList();
        }

        switch (ComboFilter.SelectedIndex)
        {
            case 0:
                break;
            case 1:
                allUsers = allUsers.OrderByDescending(x => x.username).ToList();
                break;
            case 2:
                allUsers = allUsers.OrderBy(x => x.username).ToList();
                break;
            default:
                break;
        }

        AllUsersBox.ItemsSource = allUsers;
    }

    private void LoadUsersForEditing()
    {
        var db = App.Services.GetRequiredService<dbBaza>();
        var users = db.users.ToList();
        UsersListBox.ItemsSource = users;
    }

    //     Control Manager
    private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.DataContext is user selectedUser)
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            var userToUpdate = db.users.FirstOrDefault(u => u.id == selectedUser.id);
            if (userToUpdate != null)
            {
                userToUpdate.role = comboBox.SelectedItem?.ToString();
                db.SaveChanges();

                //    Control Manager
                Get();

                //     
                ShowNotification($"  {selectedUser.full_name}   {userToUpdate.role}");
            }
        }
    }

    private async void AddUserButton_Click(object sender, RoutedEventArgs e)
    {
        var newUser = new user
        {
            username = "New",
            password = "123456",
            full_name = "New",
            email = "",
            role = "operator",
            is_active = true
        };

        var db = App.Services.GetRequiredService<dbBaza>();
        db.users.Add(newUser);
        db.SaveChanges();

        LoadUsersForEditing();

        await ShowNotificationDialog("  .   : 123456");
    }

    private async void SaveUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            var userToUpdate = db.users.FirstOrDefault(u => u.id == userId);
            if (userToUpdate != null)
            {
                //    ListBox
                if (UsersListBox.Items is IEnumerable<user> users)
                {
                    var editedUser = users.FirstOrDefault(u => u.id == userId);
                    if (editedUser != null)
                    {
                        //  
                        userToUpdate.username = editedUser.username;
                        userToUpdate.full_name = editedUser.full_name;
                        userToUpdate.email = editedUser.email;
                        userToUpdate.role = editedUser.role;
                        userToUpdate.is_active = editedUser.is_active;

                        db.SaveChanges();

                        //   
                        Get();
                        LoadUsersForEditing();

                        await ShowNotificationDialog($" {editedUser.full_name}  ");
                    }
                }
            }
        }
    }

    //   
    private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            //  
            var dialog = new Window()
            {
                Title = " ",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var panel = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20
            };

            panel.Children.Add(new TextBlock
            {
                Text = " ,     ?",
                TextWrapping = TextWrapping.Wrap
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10
            };

            var yesButton = new Button { Content = "", Width = 80 };
            var noButton = new Button { Content = "", Width = 80 };

            yesButton.Click += async (s, args) =>
            {
                var db = App.Services.GetRequiredService<dbBaza>();
                var userToDelete = db.users.FirstOrDefault(u => u.id == userId);
                if (userToDelete != null)
                {
                    // ,      
                    if (userToDelete.role == "admin")
                    {
                        var adminCount = db.users.Count(u => u.role == "admin");
                        if (adminCount <= 1)
                        {
                            await ShowNotificationDialog("   !");
                            dialog.Close();
                            return;
                        }
                    }

                    db.users.Remove(userToDelete);
                    db.SaveChanges();

                    //   
                    Get();
                    LoadUsersForEditing();

                    await ShowNotificationDialog($" {userToDelete.full_name} ");
                    dialog.Close();
                }
            };

            noButton.Click += (s, args) => dialog.Close();

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);
            panel.Children.Add(buttonPanel);
            dialog.Content = panel;

            await dialog.ShowDialog(this);
        }
    }

    //   
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        Get();
        LoadUsersForEditing();
    }

    // ----- Categories / Products (without ViewModels) -----


    private void LoadUnits()
    {
        var db = App.Services.GetRequiredService<dbBaza>();
        var items = db.units
            .AsNoTracking()
            .OrderBy(u => u.name)
            .ToList();

        _units.Clear();
        foreach (var u in items) _units.Add(u);
    }



    private async Task<bool> ConfirmAsync(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 380,
            Height = 170,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var root = new StackPanel { Margin = new Thickness(16), Spacing = 12 };
        root.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8
        };

        var ok = new Button { Content = "??", Width = 90 };
        var cancel = new Button { Content = "???", Width = 90 };

        var tcs = new TaskCompletionSource<bool>();
        ok.Click += (_, __) => { tcs.TrySetResult(true); dialog.Close(); };
        cancel.Click += (_, __) => { tcs.TrySetResult(false); dialog.Close(); };

        buttons.Children.Add(ok);
        buttons.Children.Add(cancel);
        root.Children.Add(buttons);

        dialog.Content = root;
        await dialog.ShowDialog(this);
        return await tcs.Task;
    }

    private async Task<product_category?> ShowCategoryDialogAsync(product_category? existing)
    {
        var dialog = new Window
        {
            Title = existing == null ? "????? ?????????" : "?????????????? ?????????",
            Width = 520,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var nameBox = new TextBox { Watermark = "????????", Text = existing?.name ?? "" };
        var parentBox = new ComboBox { PlaceholderText = "???????????? ????????? (?????????????)" };
        var descBox = new TextBox { Watermark = "????????", Text = existing?.description ?? "", AcceptsReturn = true, Height = 90 };

        var parentChoices = _categories
            .Where(c => existing == null || c.id != existing.id)
            .OrderBy(c => c.name)
            .ToList();
        parentBox.ItemsSource = parentChoices;
        parentBox.DisplayMemberBinding = new Avalonia.Data.Binding("name");
        if (existing?.parent_id != null)
            parentBox.SelectedItem = parentChoices.FirstOrDefault(c => c.id == existing.parent_id);

        var okBtn = new Button { Content = "?????????", Width = 120 };
        var cancelBtn = new Button { Content = "??????", Width = 120 };
        var tcs = new TaskCompletionSource<product_category?>();

        okBtn.Click += async (_, __) =>
        {
            var name = (nameBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                await ShowNotificationDialog("??????? ???????? ?????????.");
                return;
            }

            var parent = parentBox.SelectedItem as product_category;
            tcs.TrySetResult(new product_category
            {
                id = existing?.id ?? 0,
                name = name,
                parent_id = parent?.id,
                description = string.IsNullOrWhiteSpace(descBox.Text) ? null : descBox.Text.Trim()
            });
            dialog.Close();
        };

        cancelBtn.Click += (_, __) => { tcs.TrySetResult(null); dialog.Close(); };

        var form = new StackPanel { Margin = new Thickness(16), Spacing = 8 };
        form.Children.Add(nameBox);
        form.Children.Add(parentBox);
        form.Children.Add(descBox);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(16, 0, 16, 16)
        };
        buttons.Children.Add(okBtn);
        buttons.Children.Add(cancelBtn);

        var root = new DockPanel();
        DockPanel.SetDock(buttons, Dock.Bottom);
        root.Children.Add(buttons);
        root.Children.Add(form);

        dialog.Content = root;
        await dialog.ShowDialog(this);
        return await tcs.Task;
    }

    private async Task<product?> ShowProductDialogAsync(product? existing)
    {
        var dialog = new Window
        {
            Title = existing == null ? "????? ?????" : "?????????????? ??????",
            Width = 640,
            Height = 520,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var skuBox = new TextBox { Watermark = "SKU", Text = existing?.sku ?? "" };
        var nameBox = new TextBox { Watermark = "????????", Text = existing?.name ?? "" };
        var categoryBox = new ComboBox { PlaceholderText = "????????? (?????????????)" };
        var unitBox = new ComboBox { PlaceholderText = "??.??? (???????????)" };
        var barcodeBox = new TextBox { Watermark = "????????", Text = existing?.barcode ?? "" };
        var purchasePriceBox = new TextBox { Watermark = "?????????? ???? (???????? 12.50)", Text = existing?.purchase_price?.ToString() ?? "" };
        var sellingPriceBox = new TextBox { Watermark = "???? ??????? (???????? 15.00)", Text = existing?.selling_price?.ToString() ?? "" };
        var minBox = new TextBox { Watermark = "???.???????", Text = existing?.min_stock_level?.ToString() ?? "" };
        var maxBox = new TextBox { Watermark = "????.???????", Text = existing?.max_stock_level?.ToString() ?? "" };
        var isActiveBox = new CheckBox { Content = "???????", IsChecked = existing?.is_active ?? true };
        var descBox = new TextBox { Watermark = "????????", Text = existing?.description ?? "", AcceptsReturn = true, Height = 90 };

        categoryBox.ItemsSource = _categories.OrderBy(c => c.name).ToList();
        categoryBox.DisplayMemberBinding = new Avalonia.Data.Binding("name");
        if (existing?.category_id != null)
            categoryBox.SelectedItem = _categories.FirstOrDefault(c => c.id == existing.category_id);

        unitBox.ItemsSource = _units.OrderBy(u => u.name).ToList();
        unitBox.DisplayMemberBinding = new Avalonia.Data.Binding("name");
        unitBox.SelectedItem = _units.FirstOrDefault(u => u.id == existing?.unit_id) ?? _units.FirstOrDefault();

        int? ParseIntOrNull(string? s)
        {
            s = (s ?? "").Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;
            return int.TryParse(s, out var v) ? v : null;
        }

        decimal? ParseDecOrNull(string? s)
        {
            s = (s ?? "").Trim().Replace(',', '.');
            if (string.IsNullOrWhiteSpace(s)) return null;
            return decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
        }

        var okBtn = new Button { Content = "?????????", Width = 120 };
        var cancelBtn = new Button { Content = "??????", Width = 120 };
        var tcs = new TaskCompletionSource<product?>();

        okBtn.Click += async (_, __) =>
        {
            var sku = (skuBox.Text ?? "").Trim();
            var name = (nameBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name))
            {
                await ShowNotificationDialog("SKU ? ???????? ???????????.");
                return;
            }

            if (unitBox.SelectedItem is not unit selectedUnit)
            {
                await ShowNotificationDialog("???????? ??????? ?????????.");
                return;
            }

            var selectedCategory = categoryBox.SelectedItem as product_category;

            tcs.TrySetResult(new product
            {
                id = existing?.id ?? 0,
                sku = sku,
                name = name,
                description = string.IsNullOrWhiteSpace(descBox.Text) ? null : descBox.Text.Trim(),
                category_id = selectedCategory?.id,
                unit_id = selectedUnit.id,
                purchase_price = ParseDecOrNull(purchasePriceBox.Text),
                selling_price = ParseDecOrNull(sellingPriceBox.Text),
                min_stock_level = ParseIntOrNull(minBox.Text),
                max_stock_level = ParseIntOrNull(maxBox.Text),
                barcode = string.IsNullOrWhiteSpace(barcodeBox.Text) ? null : barcodeBox.Text.Trim(),
                is_active = isActiveBox.IsChecked ?? true
            });
            dialog.Close();
        };

        cancelBtn.Click += (_, __) => { tcs.TrySetResult(null); dialog.Close(); };

        var form = new StackPanel { Margin = new Thickness(16), Spacing = 8 };
        form.Children.Add(skuBox);
        form.Children.Add(nameBox);
        form.Children.Add(categoryBox);
        form.Children.Add(unitBox);
        form.Children.Add(barcodeBox);
        form.Children.Add(purchasePriceBox);
        form.Children.Add(sellingPriceBox);
        form.Children.Add(minBox);
        form.Children.Add(maxBox);
        form.Children.Add(isActiveBox);
        form.Children.Add(descBox);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(16, 0, 16, 16)
        };
        buttons.Children.Add(okBtn);
        buttons.Children.Add(cancelBtn);

        var root = new DockPanel();
        DockPanel.SetDock(buttons, Dock.Bottom);
        root.Children.Add(buttons);
        root.Children.Add(new ScrollViewer { Content = form });

        dialog.Content = root;
        await dialog.ShowDialog(this);
        return await tcs.Task;
    }

    private void RefreshProductsButton_Click(object sender, RoutedEventArgs e)
    {
        
        LoadUnits();
        
    }

    private async void AddCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        
        var model = await ShowCategoryDialogAsync(null);
        if (model == null) return;

        var db = App.Services.GetRequiredService<dbBaza>();
        db.product_categories.Add(new product_category
        {
            name = model.name,
            description = model.description,
            parent_id = model.parent_id
        });
        await db.SaveChangesAsync();
        
    }
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Close();
    }
    private async Task ShowNotificationDialog(string message)
    {
        var dialog = new Window()
        {
            Title = "",
            Width = 400,
            Height = 100,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        panel.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap
        });

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };

        okButton.Click += (s, args) => dialog.Close();
        panel.Children.Add(okButton);
        dialog.Content = panel;

        await dialog.ShowDialog(this);
    }
    private void ShowNotification(string message)
    {
        Console.WriteLine($"Notification: {message}");
    }
}