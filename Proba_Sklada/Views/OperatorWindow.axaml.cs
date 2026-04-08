using Avalonia.Controls;
using Inventori_Manager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
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
            DataContext = new InventoryViewModel();
            Get();
        }
        public OperatorWindow(user us)
        {
            InitializeComponent();
            //FioBox.Text = $"{us.username}";
            _user = us;
            DataContext = new InventoryViewModel(us);
            Get();
        }

        private void Get()
        {
            using (var db = new dbBaza())
            {
                List<inventory> inv = db.inventories.Include(e => e.location).Include(e => e.product).ToList();
            }
        }
    }
}