
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace toybox
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Product> products = new();
        private readonly ObservableCollection<CartItem> cart = new();
        private readonly ObservableCollection<Sale> sales = new();

        private readonly string[] categories =
        {
            "Boneka", "Kendaraan", "Edukasi",
            "Konstruksi", "Action Figure", "Pretend Play"
        };

        private readonly Brush navy = new SolidColorBrush(Color.FromRgb(23, 54, 84));
        private readonly Brush blue = new SolidColorBrush(Color.FromRgb(43, 91, 130));
        private readonly Brush background = new SolidColorBrush(Color.FromRgb(244, 247, 251));
        private readonly Brush yellow = new SolidColorBrush(Color.FromRgb(255, 209, 102));
        private readonly Brush green = new SolidColorBrush(Color.FromRgb(30, 145, 105));

        private DataGrid productGrid = new();
        private DataGrid stockGrid = new();
        private DataGrid cartGrid = new();
        private DataGrid reportGrid = new();

        private TextBox searchBox = new();
        private ComboBox productPicker = new();
        private TextBox quantityBox = new();
        private TextBlock cartTotalText = new();

        public MainWindow()
        {
            InitializeComponent();

            SeedProducts();
            ShowDashboard();
        }

        // =========================
        // DATA CONTOH
        // =========================

        private void SeedProducts()
        {
            products.Add(new Product
            {
                SKU = "BY001", ProductName = "Boneka Beruang",
                Category = "Boneka", PurchasePrice = 35000,
                SellingPrice = 75000, Stock = 12, MinimumStock = 5
            });

            products.Add(new Product
            {
                SKU = "KB001", ProductName = "Mobil Balap",
                Category = "Kendaraan", PurchasePrice = 20000,
                SellingPrice = 45000, Stock = 18, MinimumStock = 5
            });

            products.Add(new Product
            {
                SKU = "ED001", ProductName = "Puzzle Angka",
                Category = "Edukasi", PurchasePrice = 15000,
                SellingPrice = 35000, Stock = 3, MinimumStock = 5
            });

            products.Add(new Product
            {
                SKU = "KN001", ProductName = "Balok Susun",
                Category = "Konstruksi", PurchasePrice = 40000,
                SellingPrice = 85000, Stock = 9, MinimumStock = 4
            });

            products.Add(new Product
            {
                SKU = "AF001", ProductName = "Robot Hero",
                Category = "Action Figure", PurchasePrice = 50000,
                SellingPrice = 95000, Stock = 7, MinimumStock = 3
            });

            products.Add(new Product
            {
                SKU = "PP001", ProductName = "Kitchen Set Mini",
                Category = "Pretend Play", PurchasePrice = 45000,
                SellingPrice = 89000, Stock = 2, MinimumStock = 4
            });
        }

        // =========================
        // KOMPONEN TAMPILAN
        // =========================

        private TextBlock Text(string value, double size = 14, bool bold = false)
        {
            return new TextBlock
            {
                Text = value,
                FontSize = size,
                FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = navy,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 4)
            };
        }

        private Button MakeButton(string caption, Action action, Brush? color = null)
        {
            var button = new Button
            {
                Content = caption,
                Background = color ?? blue,
                Foreground = Brushes.White,
                Padding = new Thickness(14, 9, 14, 9),
                Margin = new Thickness(0, 0, 8, 8),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                FontSize = 13
            };

            button.Click += (_, _) => action();
            return button;
        }

        private StackPanel NewPage(string title, string subtitle)
        {
            var page = new StackPanel();

            page.Children.Add(Text(title, 29, true));

            var sub = Text(subtitle, 14);
            sub.Foreground = Brushes.Gray;
            sub.Margin = new Thickness(0, 0, 0, 20);
            page.Children.Add(sub);

            MainContent.Content = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = page
            };

            return page;
        }

        private Border Card(string title, string value, string note, Brush color)
        {
            var stack = new StackPanel();

            stack.Children.Add(Text(title, 13));
            stack.Children.Add(Text(value, 27, true));

            var description = Text(note, 12);
            description.Foreground = color;
            stack.Children.Add(description);

            return new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 12, 12),
                Child = stack,
                MinWidth = 150
            };
        }

        private DataGrid MakeGrid()
        {
            return new DataGrid
            {
                AutoGenerateColumns = true,
                IsReadOnly = true,
                CanUserAddRows = false,
                SelectionMode = DataGridSelectionMode.Single,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                Background = Brushes.White,
                RowBackground = Brushes.White,
                AlternatingRowBackground = background,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(5),
                MinHeight = 220
            };
        }

        private void RefreshProductPicker()
        {
            productPicker.ItemsSource = null;
            productPicker.ItemsSource = products.ToList();
            productPicker.DisplayMemberPath = "ProductName";
        }

        // =========================
        // NAVIGASI
        // =========================

        private void Dashboard_Click(object sender, RoutedEventArgs e) => ShowDashboard();
        private void Produk_Click(object sender, RoutedEventArgs e) => ShowProducts();
        private void Stok_Click(object sender, RoutedEventArgs e) => ShowStock();
        private void Kasir_Click(object sender, RoutedEventArgs e) => ShowCashier();
        private void Laporan_Click(object sender, RoutedEventArgs e) => ShowReports();

        // =========================
        // DASHBOARD
        // =========================

        private void ShowDashboard()
        {
            var page = NewPage("Dashboard", "Selamat datang di TOYBOX!");

            var cards = new WrapPanel();
            cards.Children.Add(Card(
                "Total Produk", products.Count.ToString(),
                "Jenis mainan terdaftar", blue));

            cards.Children.Add(Card(
                "Stok Menipis",
                products.Count(p => p.Stock <= p.MinimumStock).ToString(),
                "Perlu diperiksa", Brushes.DarkGoldenrod));

            cards.Children.Add(Card(
                "Transaksi Hari Ini",
                sales.Count(s => s.Date.Date == DateTime.Today).ToString(),
                "Transaksi selesai", green));

            cards.Children.Add(Card(
                "Pendapatan Hari Ini",
                Rupiah(sales.Where(s => s.Date.Date == DateTime.Today).Sum(s => s.Total)),
                "Total penjualan", green));

            page.Children.Add(cards);

            var welcome = new StackPanel();
            welcome.Children.Add(Text("Selamat datang di TOYBOX!", 21, true));
            welcome.Children.Add(Text(
                "Kelola produk, stok, penjualan, dan laporan toko mainan dari satu aplikasi."));
            welcome.Children.Add(Text("Stok yang perlu diperiksa:", 16, true));

            foreach (var p in products.Where(p => p.Stock <= p.MinimumStock))
                welcome.Children.Add(Text($"• {p.ProductName}: {p.Stock} unit tersisa"));

            page.Children.Add(new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(22),
                Margin = new Thickness(0, 8, 0, 0),
                Child = welcome
            });
        }

        // =========================
        // PRODUK
        // =========================

        private void ShowProducts()
        {
            var page = NewPage("Manajemen Produk", "Tambah dan kelola katalog mainan.");

            var actions = new WrapPanel();

            actions.Children.Add(MakeButton("＋ Tambah Produk", AddProduct, green));
            actions.Children.Add(MakeButton("✎ Edit Produk", EditProduct));
            actions.Children.Add(MakeButton("Hapus Produk", DeleteProduct,
                new SolidColorBrush(Color.FromRgb(190, 65, 65))));

            page.Children.Add(actions);

            searchBox = new TextBox
            {
                Height = 36,
                FontSize = 14,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 5, 0, 12),
                ToolTip = "Cari nama, SKU, atau kategori produk"
            };

            searchBox.TextChanged += (_, _) => FilterProducts();
            page.Children.Add(searchBox);

            productGrid = MakeGrid();
            productGrid.ItemsSource = products;
            page.Children.Add(productGrid);
        }

        private void FilterProducts()
        {
            if (productGrid == null) return;

            string keyword = searchBox.Text.Trim().ToLowerInvariant();

            productGrid.ItemsSource = null;
            productGrid.ItemsSource = products.Where(p =>
                p.ProductName.ToLowerInvariant().Contains(keyword) ||
                p.SKU.ToLowerInvariant().Contains(keyword) ||
                p.Category.ToLowerInvariant().Contains(keyword)).ToList();
        }

        private void AddProduct()
        {
            var p = new Product
            {
                SKU = "PR" + DateTime.Now.ToString("HHmmss"),
                Category = categories[0],
                MinimumStock = 5
            };

            if (!EditProductDialog(p, false)) return;

            if (products.Any(x => x.SKU.Equals(p.SKU, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("SKU sudah digunakan. Gunakan SKU yang berbeda.");
                return;
            }

            products.Add(p);
            ShowProducts();
        }

        private void EditProduct()
        {
            if (productGrid.SelectedItem is not Product selected)
            {
                MessageBox.Show("Pilih produk yang ingin diedit terlebih dahulu.");
                return;
            }

            var copy = new Product
            {
                SKU = selected.SKU,
                ProductName = selected.ProductName,
                Category = selected.Category,
                PurchasePrice = selected.PurchasePrice,
                SellingPrice = selected.SellingPrice,
                Stock = selected.Stock,
                MinimumStock = selected.MinimumStock
            };

            if (!EditProductDialog(copy, true)) return;

            if (products.Any(x => x != selected &&
                x.SKU.Equals(copy.SKU, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("SKU sudah digunakan produk lain.");
                return;
            }

            selected.SKU = copy.SKU;
            selected.ProductName = copy.ProductName;
            selected.Category = copy.Category;
            selected.PurchasePrice = copy.PurchasePrice;
            selected.SellingPrice = copy.SellingPrice;
            selected.Stock = copy.Stock;
            selected.MinimumStock = copy.MinimumStock;

            ShowProducts();
        }

        private void DeleteProduct()
        {
            if (productGrid.SelectedItem is not Product selected)
            {
                MessageBox.Show("Pilih produk yang ingin dihapus.");
                return;
            }

            if (MessageBox.Show(
                $"Hapus produk {selected.ProductName}?",
                "Konfirmasi",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (cart.Any(c => c.Product == selected))
            {
                MessageBox.Show("Produk masih ada di keranjang kasir. Hapus dari keranjang dahulu.");
                return;
            }

            products.Remove(selected);
            ShowProducts();
        }

        private bool EditProductDialog(Product product, bool editing)
        {
            var window = new Window
            {
                Title = editing ? "Edit Produk" : "Tambah Produk",
                Width = 420,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
                Background = background
            };

            var panel = new StackPanel { Margin = new Thickness(22) };

            panel.Children.Add(Text(editing ? "Edit Produk" : "Tambah Produk", 22, true));

            TextBox AddField(string label, string value)
            {
                panel.Children.Add(Text(label, 12, true));
                var input = new TextBox
                {
                    Text = value,
                    Padding = new Thickness(7),
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 7)
                };
                panel.Children.Add(input);
                return input;
            }

            var sku = AddField("SKU", product.SKU);
            var name = AddField("Nama Produk", product.ProductName);

            panel.Children.Add(Text("Kategori", 12, true));
            var category = new ComboBox
            {
                ItemsSource = categories,
                SelectedItem = product.Category,
                Padding = new Thickness(7),
                Margin = new Thickness(0, 0, 0, 7)
            };
            panel.Children.Add(category);

            var purchase = AddField("Harga Beli", product.PurchasePrice.ToString());
            var selling = AddField("Harga Jual", product.SellingPrice.ToString());
            var stock = AddField("Stok", product.Stock.ToString());
            var minimum = AddField("Batas Minimum Stok", product.MinimumStock.ToString());

            var save = MakeButton("Simpan", () =>
            {
                if (string.IsNullOrWhiteSpace(sku.Text) ||
                    string.IsNullOrWhiteSpace(name.Text))
                {
                    MessageBox.Show(window, "SKU dan nama produk wajib diisi.");
                    return;
                }

                if (!decimal.TryParse(purchase.Text, out decimal buy) || buy < 0 ||
                    !decimal.TryParse(selling.Text, out decimal sell) || sell < 0 ||
                    !int.TryParse(stock.Text, out int qty) || qty < 0 ||
                    !int.TryParse(minimum.Text, out int min) || min < 0)
                {
                    MessageBox.Show(window, "Harga dan stok harus berupa angka yang valid.");
                    return;
                }

                product.SKU = sku.Text.Trim();
                product.ProductName = name.Text.Trim();
                product.Category = category.SelectedItem?.ToString() ?? categories[0];
                product.PurchasePrice = buy;
                product.SellingPrice = sell;
                product.Stock = qty;
                product.MinimumStock = min;

                window.DialogResult = true;
                window.Close();
            }, green);

            panel.Children.Add(save);
            window.Content = new ScrollViewer { Content = panel };

            return window.ShowDialog() == true;
        }

        // =========================
        // MANAJEMEN STOK
        // =========================

        private void ShowStock()
        {
            var page = NewPage("Manajemen Stok", "Pantau dan sesuaikan jumlah stok barang.");

            var actions = new WrapPanel();
            actions.Children.Add(MakeButton("＋ Tambah Stok", () => AdjustStock(true), green));
            actions.Children.Add(MakeButton("− Kurangi Stok", () => AdjustStock(false),
                new SolidColorBrush(Color.FromRgb(190, 65, 65))));

            page.Children.Add(actions);

            stockGrid = MakeGrid();
            stockGrid.ItemsSource = products;
            page.Children.Add(stockGrid);

            page.Children.Add(Text(
                "Stok menipis ditandai jika jumlah stok sama dengan atau di bawah batas minimum.",
                12));
        }

        private void AdjustStock(bool increase)
        {
            if (stockGrid.SelectedItem is not Product selected)
            {
                MessageBox.Show("Pilih produk terlebih dahulu.");
                return;
            }

            var input = new Window
            {
                Title = increase ? "Tambah Stok" : "Kurangi Stok",
                Width = 330,
                Height = 210,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(Text(selected.ProductName, 17, true));
            panel.Children.Add(Text("Jumlah unit:"));

            var quantity = new TextBox { Text = "1", Padding = new Thickness(7) };
            panel.Children.Add(quantity);

            panel.Children.Add(MakeButton("Simpan", () =>
            {
                if (!int.TryParse(quantity.Text, out int amount) || amount <= 0)
                {
                    MessageBox.Show(input, "Masukkan jumlah bulat lebih besar dari nol.");
                    return;
                }

                if (!increase && amount > selected.Stock)
                {
                    MessageBox.Show(input, "Stok tidak boleh menjadi negatif.");
                    return;
                }

                selected.Stock += increase ? amount : -amount;
                input.DialogResult = true;
                input.Close();
            }, green));

            input.Content = panel;
            input.ShowDialog();
            ShowStock();
        }

        // =========================
        // KASIR
        // =========================

        private void ShowCashier()
        {
            var page = NewPage("Kasir", "Pilih barang, masukkan jumlah, lalu proses pembayaran.");

            page.Children.Add(Text("Pilih Produk", 14, true));

            productPicker = new ComboBox
            {
                ItemsSource = products.ToList(),
                DisplayMemberPath = "ProductName",
                Padding = new Thickness(8),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };

            if (products.Count > 0)
                productPicker.SelectedIndex = 0;

            page.Children.Add(productPicker);

            page.Children.Add(Text("Jumlah", 14, true));
            quantityBox = new TextBox
            {
                Text = "1",
                Padding = new Thickness(8),
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };
            page.Children.Add(quantityBox);

            var actions = new WrapPanel();
            actions.Children.Add(MakeButton("＋ Tambah ke Keranjang", AddToCart, green));
            actions.Children.Add(MakeButton("Hapus Item Terpilih", RemoveCartItem,
                new SolidColorBrush(Color.FromRgb(190, 65, 65))));

            page.Children.Add(actions);

            cartGrid = MakeGrid();
            cartGrid.ItemsSource = cart;
            page.Children.Add(cartGrid);

            cartTotalText = Text("Total: Rp0", 23, true);
            cartTotalText.Margin = new Thickness(0, 15, 0, 10);
            page.Children.Add(cartTotalText);

            page.Children.Add(MakeButton("PROSES TRANSAKSI", Checkout, green));
            UpdateCartTotal();
        }

        private void AddToCart()
        {
            if (productPicker.SelectedItem is not Product selected)
            {
                MessageBox.Show("Pilih produk terlebih dahulu.");
                return;
            }

            if (!int.TryParse(quantityBox.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Jumlah harus berupa angka bulat lebih besar dari nol.");
                return;
            }

            int existingQty = cart.Where(c => c.Product == selected).Sum(c => c.Quantity);

            if (existingQty + qty > selected.Stock)
            {
                MessageBox.Show($"Stok {selected.ProductName} tidak mencukupi. Stok tersedia: {selected.Stock}.");
                return;
            }

            var existing = cart.FirstOrDefault(c => c.Product == selected);

            if (existing == null)
                cart.Add(new CartItem { Product = selected, Quantity = qty });
            else
                existing.Quantity += qty;

            ShowCashier();
        }

        private void RemoveCartItem()
        {
            if (cartGrid.SelectedItem is not CartItem selected)
            {
                MessageBox.Show("Pilih item dalam keranjang terlebih dahulu.");
                return;
            }

            cart.Remove(selected);
            ShowCashier();
        }

        private void UpdateCartTotal()
        {
            if (cartTotalText != null)
                cartTotalText.Text = "Total: " + Rupiah(cart.Sum(c => c.Subtotal));
        }

        private void Checkout()
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Keranjang masih kosong.");
                return;
            }

            decimal total = cart.Sum(c => c.Subtotal);

            if (MessageBox.Show(
                $"Proses transaksi sebesar {Rupiah(total)}?",
                "Konfirmasi Pembayaran",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            foreach (var item in cart)
                item.Product.Stock -= item.Quantity;

            sales.Add(new Sale
            {
                Date = DateTime.Now,
                ItemCount = cart.Sum(c => c.Quantity),
                Total = total
            });

            cart.Clear();

            MessageBox.Show(
                $"Transaksi berhasil!\nTotal: {Rupiah(total)}",
                "Pembayaran Selesai",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ShowCashier();
        }

        // =========================
        // LAPORAN
        // =========================

        private void ShowReports()
        {
            var page = NewPage("Laporan Penjualan", "Ringkasan transaksi yang diproses melalui kasir.");

            page.Children.Add(Card(
                "Total Pendapatan",
                Rupiah(sales.Sum(s => s.Total)),
                "Seluruh transaksi", green));

            page.Children.Add(Card(
                "Jumlah Transaksi",
                sales.Count.ToString(),
                "Transaksi tercatat", blue));

            page.Children.Add(Card(
                "Total Unit Terjual",
                sales.Sum(s => s.ItemCount).ToString(),
                "Jumlah unit barang", green));

            reportGrid = MakeGrid();
            reportGrid.ItemsSource = sales;
            page.Children.Add(Text("Riwayat Transaksi", 18, true));
            page.Children.Add(reportGrid);
        }

        private string Rupiah(decimal amount)
        {
            return amount.ToString("C0",
                System.Globalization.CultureInfo.GetCultureInfo("id-ID"));
        }
    }

    // =========================
    // MODEL DATA
    // =========================

    public class Product
    {
        public string SKU { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Stock { get; set; }
        public int MinimumStock { get; set; }
    }

    public class CartItem
    {
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }

        public string ProductName => Product.ProductName;
        public decimal UnitPrice => Product.SellingPrice;
        public decimal Subtotal => UnitPrice * Quantity;
    }

    public class Sale
    {
        public DateTime Date { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
    }
}
