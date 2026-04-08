using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Inventori_Manager.Views;

public partial class AddItemWindow : Window
{
    public string ItemName { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public int Quantity { get; private set; } = 0;

    public AddItemWindow()
    {
        InitializeComponent();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            return;
        }

        ItemName = NameTextBox.Text;
        Location = LocationTextBox.Text;
        Position = PositionTextBox.Text;

        if (int.TryParse(QuantityTextBox.Text, out int quantity))
        {
            Quantity = quantity;
        }

        Close(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close(false);
    }
}