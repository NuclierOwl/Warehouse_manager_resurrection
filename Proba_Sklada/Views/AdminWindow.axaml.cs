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
    // Словарь изменённых пользователей
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
        LoadCategories();
        LoadProducts();
    }
    private void Loader()
    {
        Get();
        LoadUsersForEditing();
        LoadUnits();
        LoadCategories();
        LoadProducts();
    }

    private void SelectionChanged(object o, SelectionChangedEventArgs e)
    {
        Get();
        Loader();
    }

    private void TextChanged(object o, TextChangedEventArgs e)
    {
        Get();
        Loader();
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

    // Изменение роли через Control Manager
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

                // Обновляем список в Control Manager
                Get();

                // Уведомление об изменении роли
                ShowNotification($"Роль пользователя {selectedUser.full_name} изменена на {userToUpdate.role}");
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

        await ShowNotificationDialog("Добавлен новый пользователь. Пароль по умолчанию: 123456");
    }

    private async void SaveUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            var userToUpdate = db.users.FirstOrDefault(u => u.id == userId);
            if (userToUpdate != null)
            {
                // Данные из ListBox
                if (UsersListBox.Items is IEnumerable<user> users)
                {
                    var editedUser = users.FirstOrDefault(u => u.id == userId);
                    if (editedUser != null)
                    {
                        // Сохраняем изменения
                        userToUpdate.username = editedUser.username;
                        userToUpdate.full_name = editedUser.full_name;
                        userToUpdate.email = editedUser.email;
                        userToUpdate.role = editedUser.role;
                        userToUpdate.is_active = editedUser.is_active;

                        db.SaveChanges();

                        // Обновить оба списка
                        Get();
                        LoadUsersForEditing();

                        await ShowNotificationDialog($"Пользователь {editedUser.full_name} успешно сохранён");
                    }
                }
            }
        }
    }

    // Подтверждение удаления пользователя
    private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            // Диалог подтверждения
            var dialog = new Window()
            {
                Title = "Подтверждение удаления",
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
                Text = "Вы уверены, что хотите удалить этого пользователя?",
                TextWrapping = TextWrapping.Wrap
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10
            };

            var yesButton = new Button { Content = "Да", Width = 80 };
            var noButton = new Button { Content = "Нет", Width = 80 };

            yesButton.Click += async (s, args) =>
            {
                var db = App.Services.GetRequiredService<dbBaza>();
                var userToDelete = db.users.FirstOrDefault(u => u.id == userId);
                if (userToDelete != null)
                {
                    // Проверяем, не удаляем ли последнего администратора
                    if (userToDelete.role == "admin")
                    {
                        var adminCount = db.users.Count(u => u.role == "admin");
                        if (adminCount <= 1)
                        {
                            await ShowNotificationDialog("Нельзя удалить последнего администратора!");
                            dialog.Close();
                            return;
                        }
                    }

                    db.users.Remove(userToDelete);
                    db.SaveChanges();

                    // Обновить оба списка
                    Get();
                    LoadUsersForEditing();

                    await ShowNotificationDialog($"Пользователь {userToDelete.full_name} удалён");
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

    // Кнопка обновления пользователей
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

        var ok = new Button { Content = "Да", Width = 90 };
        var cancel = new Button { Content = "Отмена", Width = 90 };

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
            Title = existing == null ? "Новая категория" : "Редактирование категории",
            Width = 520,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var nameBox = new TextBox { Watermark = "Название", Text = existing?.name ?? "" };
        var parentBox = new ComboBox { PlaceholderText = "Родительская категория (необязательно)" };
        var descBox = new TextBox { Watermark = "Описание", Text = existing?.description ?? "", AcceptsReturn = true, Height = 90 };

        var parentChoices = _categories
            .Where(c => existing == null || c.id != existing.id)
            .OrderBy(c => c.name)
            .ToList();
        parentBox.ItemsSource = parentChoices;
        parentBox.DisplayMemberBinding = new Avalonia.Data.Binding("name");
        if (existing?.parent_id != null)
            parentBox.SelectedItem = parentChoices.FirstOrDefault(c => c.id == existing.parent_id);

        var okBtn = new Button { Content = "Сохранить", Width = 120 };
        var cancelBtn = new Button { Content = "Отмена", Width = 120 };
        var tcs = new TaskCompletionSource<product_category?>();

        okBtn.Click += async (_, __) =>
        {
            var name = (nameBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                await ShowNotificationDialog("Введите название категории.");
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
            Title = existing == null ? "Новый товар" : "Редактирование товара",
            Width = 640,
            Height = 520,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var skuBox = new TextBox { Watermark = "SKU", Text = existing?.sku ?? "" };
        var nameBox = new TextBox { Watermark = "Название", Text = existing?.name ?? "" };
        var categoryBox = new ComboBox { PlaceholderText = "Категория (необязательно)" };
        var unitBox = new ComboBox { PlaceholderText = "Ед.изм. (обязательно)" };
        var barcodeBox = new TextBox { Watermark = "Штрихкод", Text = existing?.barcode ?? "" };
        var purchasePriceBox = new TextBox { Watermark = "Закупочная цена (пример 12.50)", Text = existing?.purchase_price?.ToString() ?? "" };
        var sellingPriceBox = new TextBox { Watermark = "Цена продажи (пример 15.00)", Text = existing?.selling_price?.ToString() ?? "" };
        var minBox = new TextBox { Watermark = "Мин.остаток", Text = existing?.min_stock_level?.ToString() ?? "" };
        var maxBox = new TextBox { Watermark = "Макс.остаток", Text = existing?.max_stock_level?.ToString() ?? "" };
        var isActiveBox = new CheckBox { Content = "Активен", IsChecked = existing?.is_active ?? true };
        var descBox = new TextBox { Watermark = "Описание", Text = existing?.description ?? "", AcceptsReturn = true, Height = 90 };

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

        var okBtn = new Button { Content = "Сохранить", Width = 120 };
        var cancelBtn = new Button { Content = "Отмена", Width = 120 };
        var tcs = new TaskCompletionSource<product?>();

        okBtn.Click += async (_, __) =>
        {
            var sku = (skuBox.Text ?? "").Trim();
            var name = (nameBox.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name))
            {
                await ShowNotificationDialog("SKU и название обязательны.");
                return;
            }

            if (unitBox.SelectedItem is not unit selectedUnit)
            {
                await ShowNotificationDialog("Выберите единицу измерения.");
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
            Title = "Уведомление",
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


    #region Товары и категории
    private async void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        var model = await ShowProductDialogAsync(null);
        if (model == null) return;

        var db = App.Services.GetRequiredService<dbBaza>();
        db.products.Add(new product
        {
            sku = model.sku,
            name = model.name,
            description = model.description,
            category_id = model.category_id,
            unit_id = model.unit_id,
            purchase_price = model.purchase_price,
            selling_price = model.selling_price,
            min_stock_level = model.min_stock_level,
            max_stock_level = model.max_stock_level,
            barcode = model.barcode,
            is_active = model.is_active
        });
        await db.SaveChangesAsync();
        LoadProducts();
    }

    private async void EditProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsListBox.SelectedItem is not product selected)
        {
            await ShowNotificationDialog("Выберите товар для редактирования.");
            return;
        }
        var db = App.Services.GetRequiredService<dbBaza>();
        var existing = db.products.FirstOrDefault(p => p.id == selected.id);
        if (existing == null)
        {
            await ShowNotificationDialog("Товар не найден.");
            return;
        }
        var model = await ShowProductDialogAsync(existing);
        if (model == null) return;

        existing.sku = model.sku;
        existing.name = model.name;
        existing.description = model.description;
        existing.category_id = model.category_id;
        existing.unit_id = model.unit_id;
        existing.purchase_price = model.purchase_price;
        existing.selling_price = model.selling_price;
        existing.min_stock_level = model.min_stock_level;
        existing.max_stock_level = model.max_stock_level;
        existing.barcode = model.barcode;
        existing.is_active = model.is_active;
        await db.SaveChangesAsync();
        LoadProducts();
    }

    private async void DeleteProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (ProductsListBox.SelectedItem is not product selected)
        {
            await ShowNotificationDialog("Выберите товар для удаления.");
            return;
        }
        if (!await ConfirmAsync("Удаление товара", "Вы действительно хотите удалить выбранный товар?"))
            return;

        var db = App.Services.GetRequiredService<dbBaza>();
        var product = db.products.FirstOrDefault(p => p.id == selected.id);
        if (product != null)
        {
            db.products.Remove(product);
            await db.SaveChangesAsync();
            LoadProducts();
        }
    }

    private void RefreshProductsButton_Click(object sender, RoutedEventArgs e)
    {
        LoadUnits();
        LoadProducts();
    }

    private async void EditCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (CategoriesListBox.SelectedItem is not product_category selected)
        {
            await ShowNotificationDialog("Выберите категорию для редактирования.");
            return;
        }
        var db = App.Services.GetRequiredService<dbBaza>();
        var existing = db.product_categories.FirstOrDefault(c => c.id == selected.id);
        if (existing == null)
        {
            await ShowNotificationDialog("Категория не найдена.");
            return;
        }
        var model = await ShowCategoryDialogAsync(existing);
        if (model == null) return;

        existing.name = model.name;
        existing.description = model.description;
        existing.parent_id = model.parent_id;
        await db.SaveChangesAsync();
        LoadCategories();
        Loader();
    }

    private async void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (CategoriesListBox.SelectedItem is not product_category selected)
        {
            await ShowNotificationDialog("Выберите категорию для удаления.");
            return;
        }
        if (!await ConfirmAsync("Удаление категории", "Вы действительно хотите удалить выбранную категорию?"))
            return;

        var db = App.Services.GetRequiredService<dbBaza>();
        var category = db.product_categories.FirstOrDefault(c => c.id == selected.id);
        if (category != null)
        {
            db.product_categories.Remove(category);
            await db.SaveChangesAsync();
            LoadCategories();
        }
    }

    private void RefreshCategoriesButton_Click(object sender, RoutedEventArgs e)
    {
        LoadCategories();
    }

    private void LoadCategories()
    {
        var db = App.Services.GetRequiredService<dbBaza>();
        var items = db.product_categories
            .Include(c => c.parent)
            .OrderBy(e => e.id)
            .ToList();
        _categories.Clear();
        foreach (var c in items) _categories.Add(c);
        CategoriesListBox.ItemsSource = _categories;
    }

    private void LoadProducts()
    {
        var db = App.Services.GetRequiredService<dbBaza>();
        var items = db.products
            .Include(p => p.category)
            .Include(p => p.unit)
            .OrderBy(e => e.id)
            .ToList();
        _products.Clear();
        foreach (var p in items) _products.Add(p);
        ProductsListBox.ItemsSource = _products;
    }
    #endregion


    private void ShowNotification(string message)
    {
        Console.WriteLine($"Notification: {message}");
    }
}