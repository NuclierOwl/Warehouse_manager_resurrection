using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proba_Sklada.Hardik
{
    internal class Dop
    {
        public static  Window WarningWindow(string messeg, Window win)
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

           return mes;
        }
    }
}
