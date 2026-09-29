using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using Organization_Akhmarov.Data.Models;
using Organization_Akhmarov.Data.Services;

namespace Organization_Akhmarov.WPF
{
    public partial class MainWorkWindow : Window
    {
        private Organization _organization;
        private ScladService _scladService;
        private ProductService _productService;
        private InvoiceService _invoiceService;
        private Sclad _selectedSclad;
        private List<Product> _allProducts;

        public MainWorkWindow(Organization organization){
            InitializeComponent();
            _organization = organization;

            var orgService = new OrganizationService();
            _scladService = new ScladService(orgService);
            _productService = new ProductService(_scladService);
            _invoiceService = new InvoiceService(_productService);

            TxtOrgName.Text = _organization.Name;
            LoadSclads();
        }

        private void LoadSclads()
        {
            var sclads = _scladService.GetByOrganizationId(_organization.Id);
            LvSclads.ItemsSource = sclads;
        }

        private void LoadProducts(){
            if (_selectedSclad != null){
                _allProducts = _productService.GetProductsBySclad(_selectedSclad.Id).ToList();
                ApplyFilterAndSort();
                TxtScladName.Text = _selectedSclad.Name;
            }
            else
            {
                LvProducts.ItemsSource = null;
                TxtScladName.Text = "[не выбран]";
            }
        }

        private void ApplyFilterAndSort()
        {
            if (_allProducts == null) return;

            var query = _allProducts.AsEnumerable();

            // Поиск
            string searchText = TxtSearch.Text;
            if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск по наименованию.")
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchText.ToLower()));
            }

            // Сортировка
            var sortItem = (CmbSort.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (sortItem){
                case "По цене (возр.)":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "По цене (убыв.)":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "По остатку":
                    query = query.OrderByDescending(p => p.StockQuantity);
                    break;
                case "По скидке":
                    query = query.OrderByDescending(p => p.DiscountPercent);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            LvProducts.ItemsSource = query.ToList();
        }

        private void LvSclads_SelectionChanged(object sender, SelectionChangedEventArgs e){
            _selectedSclad = LvSclads.SelectedItem as Sclad;
            LoadProducts();
        }

        private void BtnAddSclad_Click(object sender, RoutedEventArgs e){
            var dialog = new ScladDialog();
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ScladName)){
                _scladService.Add(new Sclad(dialog.ScladName, dialog.ScladAddress, _organization.Id));
                LoadSclads();
            }
        }

        private void BtnEditSclad_Click(object sender, RoutedEventArgs e){
            if (_selectedSclad == null){
                MessageBox.Show("Выберите склад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ScladDialog(_selectedSclad.Name, _selectedSclad.Address);
            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.ScladName))
            {
                _selectedSclad.Name = dialog.ScladName;
                _selectedSclad.Address = dialog.ScladAddress;
                _scladService.Update(_selectedSclad);
                LoadSclads();
            }
        }

        private void BtnDeleteSclad_Click(object sender, RoutedEventArgs e){
            if (_selectedSclad == null){
                MessageBox.Show("Выберите склад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить склад \"{_selectedSclad.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _scladService.Delete(_selectedSclad.Id);
                _selectedSclad = null;
                LoadSclads();
                LoadProducts();
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e){
            if (_selectedSclad == null){
                MessageBox.Show("Выберите склад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ProductDialog(_productService.GetAllCategories(),
                                           _productService.GetAllManufacturers(),
                                           _productService.GetAllSuppliers());
            if (dialog.ShowDialog() == true && dialog.Product != null)
            {
                _productService.Add(dialog.Product);
                LoadProducts();
                MessageBox.Show($"Товар \"{dialog.Product.Name}\" добавлен", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e){
            var selected = LvProducts.SelectedItem as Product;
            if (selected == null){
                MessageBox.Show("Выберите товар", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ProductDialog(_productService.GetAllCategories(),
                                           _productService.GetAllManufacturers(),
                                           _productService.GetAllSuppliers(), selected);
            if (dialog.ShowDialog() == true && dialog.Product != null)
            {
                _productService.Update(dialog.Product);
                LoadProducts();
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e){
            var selected = LvProducts.SelectedItem as Product;
            if (selected == null){
                MessageBox.Show("Выберите товар", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Удалить товар \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _productService.Delete(selected.Article);
                LoadProducts();
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e){
            var dialog = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv" };
            if (dialog.ShowDialog() == true){
                try{
                    var lines = System.IO.File.ReadAllLines(dialog.FileName);
                    int imported = 0;
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var parts = lines[i].Split(',');
                        if (parts.Length >= 3)
                        {
                            var product = new Product
                            {
                                Name = parts[0].Trim(),
                                Price = decimal.TryParse(parts[1], out var p) ? p : 0,
                                StockQuantity = int.TryParse(parts[2], out var s) ? s : 0
                            };
                            _productService.Add(product);
                            imported++;
                        }
                    }
                    LoadProducts();
                    MessageBox.Show($"Импортировано: {imported}", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex){
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnInvoiceIn_Click(object sender, RoutedEventArgs e){
            if (_selectedSclad == null){
                MessageBox.Show("Выберите склад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new InvoiceDialog(_productService.GetAllProducts(), InvoiceType.Incoming);
            if (dialog.ShowDialog() == true && dialog.Invoice != null)
            {
                dialog.Invoice.ScladId = _selectedSclad.Id;
                if (_invoiceService.ConfirmInvoice(dialog.Invoice))
                {
                    LoadProducts();
                    MessageBox.Show("Приходная накладная проведена", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnInvoiceOut_Click(object sender, RoutedEventArgs e){
            if (_selectedSclad == null){
                MessageBox.Show("Выберите склад", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new InvoiceDialog(_productService.GetAllProducts(), InvoiceType.Outgoing);
            if (dialog.ShowDialog() == true && dialog.Invoice != null){
                dialog.Invoice.ScladId = _selectedSclad.Id;
                if (_invoiceService.ConfirmInvoice(dialog.Invoice)){
                    LoadProducts();
                    MessageBox.Show("Расходная накладная проведена", "Успешно",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Недостаточно товара", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e){
            ApplyFilterAndSort();}

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e){
            ApplyFilterAndSort();}

        private void BtnBack_Click(object sender, RoutedEventArgs e){
            var orgWindow = new OrganizationsWindow();
            orgWindow.Show();
            this.Close();
        }
    }
}