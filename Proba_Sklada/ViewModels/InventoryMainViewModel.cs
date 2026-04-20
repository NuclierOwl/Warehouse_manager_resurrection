using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using Proba_Sklada.Hardik.Connector;
using Proba_Sklada.Hardik.Dao;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventori_Manager.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly dbBaza _context;
        private readonly user _currentUser;

        #region Коллекции для выпадающих списков
        public ObservableCollection<product> Products { get; set; }
        public ObservableCollection<storage_location> StorageLocations { get; set; }
        public ObservableCollection<counter_ogent> Suppliers { get; set; }
        public ObservableCollection<counter_ogent> Customers { get; set; }
        public ObservableCollection<inventory> InventoryItems { get; set; }
        public ObservableCollection<inventory> Inventory { get; set; }
        public ObservableCollection<LocationStockModel> LocationStocks { get; set; }
        #endregion

        #region Приход
        private string _newInvoiceNumber;
        private DateTimeOffset? _newInvoiceDate;
        private counter_ogent _selectedSupplier;
        private product _selectedProduct;
        private decimal _quantity;
        private storage_location _selectedLocation;
        private string _batchNumber;
        private ObservableCollection<InvoiceItemModel> _currentInvoiceItems;

        public string NewInvoiceNumber { get => _newInvoiceNumber; set { _newInvoiceNumber = value; OnPropertyChanged(); } }
        public DateTimeOffset? NewInvoiceDate { get => _newInvoiceDate; set { _newInvoiceDate = value; OnPropertyChanged(); } }
        public counter_ogent SelectedSupplier { get => _selectedSupplier; set { _selectedSupplier = value; OnPropertyChanged(); } }
        public product SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(); } }
        public decimal Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); } }
        public storage_location SelectedLocation { get => _selectedLocation; set { _selectedLocation = value; OnPropertyChanged(); } }
        public string BatchNumber { get => _batchNumber; set { _batchNumber = value; OnPropertyChanged(); } }
        public ObservableCollection<InvoiceItemModel> CurrentInvoiceItems { get => _currentInvoiceItems; set { _currentInvoiceItems = value; OnPropertyChanged(); } }
        #endregion

        #region Инвентаризация
        private product _selectedProductInv;
        private storage_location _selectedLocationInv;
        private decimal _actualQuantity;
        private ObservableCollection<DiscrepancyModel> _discrepancies;

        public product SelectedProductInv { get => _selectedProductInv; set { _selectedProductInv = value; OnPropertyChanged(); } }
        public storage_location SelectedLocationInv { get => _selectedLocationInv; set { _selectedLocationInv = value; OnPropertyChanged(); } }
        public decimal ActualQuantity { get => _actualQuantity; set { _actualQuantity = value; OnPropertyChanged(); } }
        public ObservableCollection<DiscrepancyModel> Discrepancies { get => _discrepancies; set { _discrepancies = value; OnPropertyChanged(); } }
        #endregion

        #region Списание
        private string _expenseInvoiceNumber;
        private DateTimeOffset? _expenseInvoiceDate;
        private counter_ogent _selectedCustomer;
        private product _selectedProductExp;
        private decimal _expenseQuantity;
        private storage_location _selectedLocationExp;
        private decimal _expenseUnitPrice;
        private ObservableCollection<ExpenseItemModel> _currentExpenseItems;
        public string ExpenseInvoiceNumber { get => _expenseInvoiceNumber; set { _expenseInvoiceNumber = value; OnPropertyChanged(); } }
        public DateTimeOffset? ExpenseInvoiceDate { get => _expenseInvoiceDate; set { _expenseInvoiceDate = value; OnPropertyChanged(); } }
        public counter_ogent SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(); } }
        public product SelectedProductExp { get => _selectedProductExp; set { _selectedProductExp = value; OnPropertyChanged(); } }
        public decimal ExpenseQuantity { get => _expenseQuantity; set { _expenseQuantity = value; OnPropertyChanged(); } }
        public storage_location SelectedLocationExp { get => _selectedLocationExp; set { _selectedLocationExp = value; OnPropertyChanged(); } }
        public decimal ExpenseUnitPrice { get => _expenseUnitPrice; set { _expenseUnitPrice = value; OnPropertyChanged(); } }
        public ObservableCollection<ExpenseItemModel> CurrentExpenseItems { get => _currentExpenseItems; set { _currentExpenseItems = value; OnPropertyChanged(); } }
        #endregion

        #region Команды
        public ICommand AddToInvoiceCommand { get; }
        public ICommand SaveInvoiceCommand { get; }
        public ICommand ClearInvoiceCommand { get; }
        public ICommand RecordDiscrepancyCommand { get; }
        public ICommand AddToExpenseCommand { get; }
        public ICommand SaveExpenseCommand { get; }
        public ICommand ClearExpenseCommand { get; }
        #endregion

        #region Позиции (фильтрация)
        private ObservableCollection<inventory> _filteredInventoryItems = new();
        private product? _filterProduct;
        private storage_location? _filterLocation;
        private string _filterBatchNumber = string.Empty;
        private bool _filterOnlyPositiveQuantity = true;

        public ObservableCollection<inventory> FilteredInventoryItems
        {
            get => _filteredInventoryItems;
            set { _filteredInventoryItems = value; OnPropertyChanged(); }
        }

        public product? FilterProduct
        {
            get => _filterProduct;
            set { _filterProduct = value; OnPropertyChanged(); }
        }

        public storage_location? FilterLocation
        {
            get => _filterLocation;
            set { _filterLocation = value; OnPropertyChanged(); }
        }

        public string FilterBatchNumber
        {
            get => _filterBatchNumber;
            set { _filterBatchNumber = value; OnPropertyChanged(); }
        }

        public bool FilterOnlyPositiveQuantity
        {
            get => _filterOnlyPositiveQuantity;
            set { _filterOnlyPositiveQuantity = value; OnPropertyChanged(); }
        }

        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }
        #endregion
        public InventoryViewModel(dbBaza context, user currentUser = null)
        {
            _context = context;
            _currentUser = currentUser;

            Products = new ObservableCollection<product>();
            StorageLocations = new ObservableCollection<storage_location>();
            Suppliers = new ObservableCollection<counter_ogent>();
            Customers = new ObservableCollection<counter_ogent>();
            InventoryItems = new ObservableCollection<inventory>();
            LocationStocks = new ObservableCollection<LocationStockModel>();

            CurrentInvoiceItems = new ObservableCollection<InvoiceItemModel>();
            Discrepancies = new ObservableCollection<DiscrepancyModel>();
            CurrentExpenseItems = new ObservableCollection<ExpenseItemModel>();

            AddToInvoiceCommand = new RelayCommand(AddToInvoice);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice);
            ClearInvoiceCommand = new RelayCommand(ClearInvoice);
            RecordDiscrepancyCommand = new RelayCommand(RecordDiscrepancy);
            AddToExpenseCommand = new RelayCommand(AddToExpense);
            SaveExpenseCommand = new RelayCommand(SaveExpense);
            ClearExpenseCommand = new RelayCommand(ClearExpense);

            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ResetFilterCommand = new RelayCommand(ResetFilter);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                await _context.Database.CanConnectAsync();

                Products.Clear();
                foreach (var p in await _context.products.Where(p => p.is_active == true).ToListAsync())
                    Products.Add(p);

                StorageLocations.Clear();
                foreach (var loc in await _context.storage_locations.ToListAsync())
                    StorageLocations.Add(loc);

                Suppliers.Clear();
                foreach (var sup in await _context.counter_ogents.Where(c => c.type == "supplier" && c.is_active == true).ToListAsync())
                    Suppliers.Add(sup);

                Customers.Clear();
                foreach (var cust in await _context.counter_ogents.Where(c => c.type == "customer" && c.is_active == true).ToListAsync())
                    Customers.Add(cust);

                InventoryItems.Clear();
                foreach (var inv in await _context.inventories.Include(i => i.product).Include(i => i.location).ToListAsync())
                    InventoryItems.Add(inv);

                ApplyFilter();
                RebuildLocationStocks();

                Discrepancies.Clear();
                var discrepancies = await _context.inventory_discrepancies
                    .Include(d => d.product)
                    .Include(d => d.location)
                    .OrderByDescending(d => d.id)
                    .ToListAsync();

                foreach (var d in discrepancies)
                {
                    Discrepancies.Add(new DiscrepancyModel
                    {
                        ProductName = d.product?.name,
                        LocationCode = d.location?.location_code,
                        ExpectedQuantity = d.expected_quantity,
                        ActualQuantity = d.actual_quantity,
                        Discrepancy = d.discrepancy ?? (d.actual_quantity - d.expected_quantity),
                        Reason = d.discrepancy_reason
                    });
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Ошибка загрузки данных", ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok)
                    .ShowAsync();
            }
        }

        private void RebuildLocationStocks()
        {
            LocationStocks.Clear();

            var grouped = InventoryItems
                .Where(i => i.location != null)
                .GroupBy(i => i.location.location_code)
                .OrderBy(g => g.Key);

            foreach (var locGroup in grouped)
            {
                var byProduct = locGroup
                    .Where(x => x.product != null)
                    .GroupBy(x => x.product.name)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Name = g.Key, Qty = g.Sum(x => x.kolichestvo) })
                    .Where(x => x.Qty != 0)
                    .ToList();

                var sb = new StringBuilder();
                sb.AppendLine($"Позиция: {locGroup.Key}");
                foreach (var p in byProduct)
                    sb.AppendLine($"{p.Name}: {p.Qty}");

                LocationStocks.Add(new LocationStockModel
                {
                    LocationCode = locGroup.Key,
                    TotalQuantity = locGroup.Sum(x => x.kolichestvo),
                    TooltipText = sb.ToString().TrimEnd()
                });
            }
        }

        // ----- Приход -----
        private void AddToInvoice()
        {
            if (SelectedProduct == null || SelectedLocation == null || Quantity <= 0)
                return;

            CurrentInvoiceItems.Add(new InvoiceItemModel
            {
                ProductId = SelectedProduct.id,
                ProductName = SelectedProduct.name,
                Quantity = Quantity,
                UnitPrice = 0, // добавить поле цены
                TotalPrice = 0,
                LocationId = SelectedLocation.id,
                LocationCode = SelectedLocation.location_code,
                BatchNumber = BatchNumber
            });

            SelectedProduct = null;
            Quantity = 0;
            SelectedLocation = null;
            BatchNumber = null;
        }

        private async void SaveInvoice()
        {
            if (string.IsNullOrWhiteSpace(NewInvoiceNumber) || SelectedSupplier == null || CurrentInvoiceItems.Count == 0)
                return;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var invoice = new postuplenium
                    {
                        invoice_number = NewInvoiceNumber,
                        invoice_date = NewInvoiceDate.HasValue ? DateOnly.FromDateTime(NewInvoiceDate.Value.DateTime) : DateOnly.FromDateTime(DateTime.Now),
                        supplier_id = SelectedSupplier.id,
                        status = "received",
                        created_by = _currentUser.id, // текущий пользователь
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    try
                    {
                        _context.postuplenia.Add(invoice);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        await MessageBoxManager.GetMessageBoxStandard("Внимание", $"У вас нет прав и {ex.ToString()}", MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                        throw;
                    }
                    decimal total = 0;
                    foreach (var item in CurrentInvoiceItems)
                    {
                        var postItem = new postuplenia_item
                        {
                            invoice_id = invoice.id,
                            product_id = item.ProductId,
                            quantity = (int)item.Quantity,
                            unit_price = item.UnitPrice,
                            location_id = item.LocationId,
                            batch_number = item.BatchNumber,
                            // total_price вычисляется автоматически, можно не заполнять
                        };
                        _context.postuplenia_items.Add(postItem);

                        var inventoryItem = await _context.inventories
                            .FirstOrDefaultAsync(i => i.product_id == item.ProductId && i.location_id == item.LocationId &&
                                                       i.batch_number == item.BatchNumber);
                        if (inventoryItem == null)
                        {
                            inventoryItem = new inventory
                            {
                                product_id = item.ProductId,
                                location_id = item.LocationId,
                                kolichestvo = (int)item.Quantity,
                                batch_number = item.BatchNumber,
                                last_updated = DateTime.Now
                            };
                            _context.inventories.Add(inventoryItem);
                        }
                        else
                        {
                            inventoryItem.kolichestvo += (int)item.Quantity;
                            inventoryItem.last_updated = DateTime.Now;
                        }

                        total += item.TotalPrice;
                    }

                    invoice.total_amount = total;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    LoadData();
                    ClearInvoice();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private void ClearInvoice()
        {
            NewInvoiceNumber = null;
            NewInvoiceDate = null;
            SelectedSupplier = null;
            CurrentInvoiceItems.Clear();
        }

        

        private void ApplyFilter()
        {
            if (InventoryItems == null) return;

            var query = InventoryItems.AsEnumerable();

            if (FilterProduct != null)
                query = query.Where(i => i.product_id == FilterProduct.id);

            if (FilterLocation != null)
                query = query.Where(i => i.location_id == FilterLocation.id);

            if (!string.IsNullOrWhiteSpace(FilterBatchNumber))
            {
                string filter = FilterBatchNumber.Trim();
                query = query.Where(i =>
                    (i.batch_number != null && i.batch_number.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                    (i.serial_number != null && i.serial_number.Contains(filter, StringComparison.OrdinalIgnoreCase)));
            }

            if (FilterOnlyPositiveQuantity)
                query = query.Where(i => i.kolichestvo > 0);

            FilteredInventoryItems = new ObservableCollection<inventory>(query);
        }

        private void ResetFilter()
        {
            FilterProduct = null;
            FilterLocation = null;
            FilterBatchNumber = string.Empty;
            FilterOnlyPositiveQuantity = true;
            ApplyFilter();
        }

        // ----- Инвентаризация -----
        private async void RecordDiscrepancy()
        {
            if (SelectedProductInv == null || SelectedLocationInv == null || ActualQuantity < 0)
                return;

            var inventoryItem = await _context.inventories
                .FirstOrDefaultAsync(i => i.product_id == SelectedProductInv.id && i.location_id == SelectedLocationInv.id);

            if (inventoryItem == null)
            {
                // Нет записи об остатках - не с чем сравнивать
                var res = MessageBoxManager.GetMessageBoxStandard("Внимание", "Нет записи об остатках - не с чем сравнивать", MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                ; return;
            }

            int expected = inventoryItem.kolichestvo;
            int actual = (int)ActualQuantity;

            if (expected == actual)
                return;

            var discrepancy = new inventory_discrepancy
            {
                inventory_id = inventoryItem.id,
                product_id = SelectedProductInv.id,
                location_id = SelectedLocationInv.id,
                expected_quantity = expected,
                actual_quantity = actual,
                discrepancy = actual - expected,
                discrepancy_reason = null,
                resolved = false,
            };
            _context.inventory_discrepancies.Add(discrepancy);
            await _context.SaveChangesAsync();

            Discrepancies.Add(new DiscrepancyModel
            {
                ProductName = SelectedProductInv.name,
                LocationCode = SelectedLocationInv.location_code,
                ExpectedQuantity = expected,
                ActualQuantity = actual,
                Discrepancy = actual - expected,
                Reason = null
            });

            SelectedProductInv = null;
            SelectedLocationInv = null;
            ActualQuantity = 0;
        }

        // ----- Списание -----
        private async void AddToExpense()
        {
            if (SelectedProductExp == null || SelectedLocationExp == null || ExpenseQuantity <= 0 || ExpenseUnitPrice <= 0)
                return;

            var inventoryItem = InventoryItems.FirstOrDefault(i => i.product_id == SelectedProductExp.id && i.location_id == SelectedLocationExp.id);
            if (inventoryItem == null || inventoryItem.kolichestvo < (int)ExpenseQuantity)
            {
                await MessageBoxManager.GetMessageBoxStandard("Ошибка при сохранении расходной накладной", "Этот товар закончился", MsBox.Avalonia.Enums.ButtonEnum.Ok).ShowAsync();
                return;
            }

            CurrentExpenseItems.Add(new ExpenseItemModel
            {
                ProductId = SelectedProductExp.id,
                ProductName = SelectedProductExp.name,
                Quantity = ExpenseQuantity,
                UnitPrice = ExpenseUnitPrice,
                TotalPrice = ExpenseQuantity * ExpenseUnitPrice,
                LocationId = SelectedLocationExp.id,
                LocationCode = SelectedLocationExp.location_code
            });

            SelectedProductExp = null;
            ExpenseQuantity = 0;
            SelectedLocationExp = null;
            ExpenseUnitPrice = 0;
        }

        private async void SaveExpense()
        {
            var ran = new Random();
            if (SelectedCustomer == null || CurrentExpenseItems.Count == 0)
                return;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var invoice = new schet_faktura
                    {
                        invoice_number = ExpenseInvoiceNumber ?? ran.Next(100000000,999999999).ToString(),
                        invoice_date = ExpenseInvoiceDate.HasValue ? DateOnly.FromDateTime(ExpenseInvoiceDate.Value.DateTime) : DateOnly.FromDateTime(DateTime.Now),
                        customer_id = SelectedCustomer.id,
                        status = "completed",
                        created_by = _currentUser?.id ?? 0,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    _context.schet_fakturas.Add(invoice);
                    await _context.SaveChangesAsync();

                    decimal total = 0;
                    foreach (var item in CurrentExpenseItems)
                    {
                        var expenseItem = new schet_faktura_soderzanie
                        {
                            invoice_id = invoice.id,
                            product_id = item.ProductId,
                            quantity = (int)item.Quantity,
                            unit_price = item.UnitPrice,
                            // total_price вычисляется автоматически
                            picked_quantity = (int)item.Quantity,
                            picked_at = DateTime.Now,
                            picked_by = 1
                        };
                        _context.schet_faktura_soderzanies.Add(expenseItem);

                        var inventoryItem = await _context.inventories
                            .FirstOrDefaultAsync(i => i.product_id == item.ProductId && i.location_id == item.LocationId);
                        if (inventoryItem != null)
                        {
                            inventoryItem.kolichestvo -= (int)item.Quantity;
                            inventoryItem.last_updated = DateTime.Now;
                        }

                        total += item.TotalPrice;
                    }

                    invoice.total_amount = total;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    LoadData();
                    ClearExpense();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    MessageBoxManager.GetMessageBoxStandard("Ошибка при сохранении расходной накладной", ex.ToString(), MsBox.Avalonia.Enums.ButtonEnum.Ok);
                    throw;
                }
            }
        }

        private void ClearExpense()
        {
            ExpenseInvoiceNumber = null;
            ExpenseInvoiceDate = null;
            SelectedCustomer = null;
            CurrentExpenseItems.Clear();
        }

        #region Вспомогательные модели
        public class LocationStockModel : INotifyPropertyChanged
        {
            private string _locationCode;
            private int _totalQuantity;
            private string _tooltipText;

            public string LocationCode { get => _locationCode; set { _locationCode = value; OnPropertyChanged(); } }
            public int TotalQuantity { get => _totalQuantity; set { _totalQuantity = value; OnPropertyChanged(); } }
            public string TooltipText { get => _tooltipText; set { _tooltipText = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class InvoiceItemModel : INotifyPropertyChanged
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public int LocationId { get; set; }
            public string LocationCode { get; set; }
            public string BatchNumber { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class DiscrepancyModel : INotifyPropertyChanged
        {
            public string ProductName { get; set; }
            public string LocationCode { get; set; }
            public double ExpectedQuantity { get; set; }
            public double ActualQuantity { get; set; }
            public double Discrepancy { get; set; }
            public string Reason { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ExpenseItemModel : INotifyPropertyChanged
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public int LocationId { get; set; }
            public string LocationCode { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
        #endregion

    // Реализация ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }

    public static class CommandManager
    {
        public static event EventHandler RequerySuggested;
        public static void InvalidateRequerySuggested() => RequerySuggested?.Invoke(null, EventArgs.Empty);
    }
}