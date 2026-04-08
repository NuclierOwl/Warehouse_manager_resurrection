using Avalonia.Controls;

namespace Proba_Sklada
{
    public partial class SkanerWindow : Window
    {
        /*
        private dbBaza _context;
        private ObservableCollection<inventory> _products;
        private ObservableCollection<string> _generatedBarcodes;
        private WriteableBitmap _currentBarcode;

        public SkanerWindow()
        {
            InitializeComponent();
            _context = new dbBaza();
            _products = new ObservableCollection<inventory>();
            _generatedBarcodes = new ObservableCollection<string>();

            LoadProducts();
            DataContext = this;
        }

        // Свойства для привязки данных
        public ObservableCollection<inventory> Products => _products;
        public ObservableCollection<string> GeneratedBarcodes => _generatedBarcodes;

        public inventory SelectedProduct { get; set; }
        public string BarcodeText { get; set; }

        private void LoadProducts()
        {
            try
            {
                var products = _context.inventories.ToList();
                _products.Clear();
                foreach (var product in products)
                {
                    _products.Add(product);
                }

                ProductComboBox.ItemsSource = _products;
                GeneratedBarcodesListBox.ItemsSource = _generatedBarcodes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void GenerateBarcode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string barcodeData = "";

                if (SelectedProduct != null && !string.IsNullOrEmpty(SelectedProduct.product.name))
                {
                    barcodeData = SelectedProduct.product.name;
                }
                else if (!string.IsNullOrEmpty(BarcodeText))
                {
                    barcodeData = BarcodeText;
                }
                else
                {
                    return;
                }

                // Генерация штрих-кода с использованием PixelData
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        Margin = 10
                    }
                };

                var pixelData = barcodeWriter.Write(barcodeData);
                _currentBarcode = CreateBitmapFromPixelData(pixelData);
                BarcodeImage.Source = _currentBarcode;

                string barcodeInfo = $"{DateTime.Now:HH:mm:ss} - {barcodeData}";
                if (!_generatedBarcodes.Contains(barcodeInfo))
                {
                    _generatedBarcodes.Add(barcodeInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации штрих-кода: {ex.Message}");
            }
        }

        private WriteableBitmap CreateBitmapFromPixelData(PixelData pixelData)
        {
            var bitmap = new WriteableBitmap(
                new PixelSize(pixelData.Width, pixelData.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);

            using (var lockedBitmap = bitmap.Lock())
            {
                var buffer = new byte[pixelData.Width * pixelData.Height * 4];

                for (int y = 0; y < pixelData.Height; y++)
                {
                    for (int x = 0; x < pixelData.Width; x++)
                    {
                        int index = y * pixelData.Width + x;
                        int pixel = pixelData.Pixels[index];

                        // Конвертируем RGB в BGRA
                        byte r = (byte)((pixel >> 16) & 0xFF);
                        byte g = (byte)((pixel >> 8) & 0xFF);
                        byte b = (byte)(pixel & 0xFF);
                        byte a = 0xFF;

                        int bufferIndex = index * 4;
                        buffer[bufferIndex] = b;     // Blue
                        buffer[bufferIndex + 1] = g; // Green
                        buffer[bufferIndex + 2] = r; // Red
                        buffer[bufferIndex + 3] = a; // Alpha
                    }
                }

                // Копируем данные в bitmap
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, lockedBitmap.Address, buffer.Length);
            }

            return bitmap;
        }

        private void SaveBarcode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentBarcode == null)
                {
                    return;
                }

                string fileName = "";
                if (SelectedProduct != null && !string.IsNullOrEmpty(SelectedProduct.product.name))
                {
                    string safeName = string.Join("_", SelectedProduct.product.name.Split(Path.GetInvalidFileNameChars()));
                    fileName = $"штрихкод_{safeName}_{DateTime.Now:yyyyMMddHHmmss}.png";
                }
                else
                {
                    fileName = $"штрихкод_{DateTime.Now:yyyyMMddHHmmss}.png";
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fullPath = Path.Combine(desktopPath, fileName);

                _currentBarcode.Save(fullPath);
                _generatedBarcodes.Add($"СОХРАНЕНО: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения штрих-кода: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            _currentBarcode?.Dispose();
            base.OnClosed(e);
        }
        */
    }

}
