using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Proba_Sklada;

namespace Inventori_Manager;

public partial class AdminWindow : Window
{
    // ˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜˜
    private Dictionary<int, user> modifiedUsers = new Dictionary<int, user>();

    public AdminWindow()
    {
        InitializeComponent();
        Loaded += AdminWindow_Loaded;
    }

    private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Get();
        LoadUsersForEditing();
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

    // ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜ ˜˜˜˜ ˜ Control Manager
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

                // ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜ Control Manager
                Get();

                // ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜
                ShowNotification($"˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜ {selectedUser.full_name} ˜˜˜˜˜˜˜˜ ˜˜ {userToUpdate.role}");
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

        await ShowNotificationDialog("˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜. ˜˜˜˜˜˜ ˜˜ ˜˜˜˜˜˜˜˜˜: 123456");
    }

    private async void SaveUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            var db = App.Services.GetRequiredService<dbBaza>();
            var userToUpdate = db.users.FirstOrDefault(u => u.id == userId);
            if (userToUpdate != null)
            {
                // ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜ ListBox
                if (UsersListBox.Items is IEnumerable<user> users)
                {
                    var editedUser = users.FirstOrDefault(u => u.id == userId);
                    if (editedUser != null)
                    {
                        // ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜
                        userToUpdate.username = editedUser.username;
                        userToUpdate.full_name = editedUser.full_name;
                        userToUpdate.email = editedUser.email;
                        userToUpdate.role = editedUser.role;
                        userToUpdate.is_active = editedUser.is_active;

                        db.SaveChanges();

                        // ˜˜˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜
                        Get();
                        LoadUsersForEditing();

                        await ShowNotificationDialog($"˜˜˜˜˜˜˜˜˜˜˜˜ {editedUser.full_name} ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜");
                    }
                }
            }
        }
    }

    // ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜
    private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int userId)
        {
            // ˜˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜
            var dialog = new Window()
            {
                Title = "˜˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜",
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
                Text = "˜˜ ˜˜˜˜˜˜˜, ˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜?",
                TextWrapping = TextWrapping.Wrap
            });

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 10
            };

            var yesButton = new Button { Content = "˜˜", Width = 80 };
            var noButton = new Button { Content = "˜˜˜", Width = 80 };

            yesButton.Click += async (s, args) =>
            {
                var db = App.Services.GetRequiredService<dbBaza>();
                var userToDelete = db.users.FirstOrDefault(u => u.id == userId);
                if (userToDelete != null)
                {
                    // ˜˜˜˜˜˜˜˜˜, ˜˜ ˜˜˜˜˜˜˜˜ ˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜
                    if (userToDelete.role == "admin")
                    {
                        var adminCount = db.users.Count(u => u.role == "admin");
                        if (adminCount <= 1)
                        {
                            await ShowNotificationDialog("˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜˜˜!");
                            dialog.Close();
                            return;
                        }
                    }

                    db.users.Remove(userToDelete);
                    db.SaveChanges();

                    // ˜˜˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜
                    Get();
                    LoadUsersForEditing();

                    await ShowNotificationDialog($"˜˜˜˜˜˜˜˜˜˜˜˜ {userToDelete.full_name} ˜˜˜˜˜˜");
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

    // ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        Get();
        LoadUsersForEditing();
    }

    // ˜˜˜˜˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜
    private string HashPassword(string password)
    {
        // ˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜
        // ˜˜˜˜˜˜˜˜: BCrypt.Net.BCrypt.HashPassword(password);
        // ˜˜˜ ˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜˜˜
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }

    // ˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜
    private async Task ShowNotificationDialog(string message)
    {
        var dialog = new Window()
        {
            Title = "˜˜˜˜˜˜˜˜˜˜˜",
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