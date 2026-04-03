using DemExamApp1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemExamApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private Frame _frame;
        private Product _product;
        private List<Product> _products;
        private User _currentUser;

        public ProductPage(Frame frame, User user)
        {
            InitializeComponent();
            _frame = frame;
            _currentUser = user;    
            LoadUser();
            LoadProducts();
            LoadSuppliers();

            SupplierFilterBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
        }

        private void LoadUser()
        {
            UserNameText.Text = _currentUser == null
                ? "Гость" 
                : _currentUser.FullName;

            if (_currentUser != null && (_currentUser.Role.Name == "Администратор" 
                || _currentUser.Role.Name == "Менеджер"))
            {
                AddProductButton.Visibility = Visibility.Visible;
            }
            else
            {
                AddProductButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadProducts()
        {
            using var db = new Dem2303Context();

            _products = db.Products
                .Include(x => x.Manufacturer)
                .Include(x => x.Supplier)
                .Include(x => x.Category)
                .ToList();

            ProductListView.ItemsSource = _products;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null && _currentUser.Role.Name == "Администратор")
            {
                _frame.Navigate(new EditProductPage(_frame, 0));
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            IEnumerable<Product> result = _products;

            string search = SearchTextBox.Text; 

            if (!string.IsNullOrEmpty(search))
            {
                result = result.Where(p => 
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Discription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Manufacturer.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Arcticle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Supplier.Name.Contains(search, StringComparison.OrdinalIgnoreCase) 
                );
            }

            if(SupplierFilterBox.SelectedIndex > 0)
            {
                string supplier = SupplierFilterBox.SelectedItem.ToString();
                result = result.Where(p => p.Supplier.Name == supplier);
            }

            switch(SortBox.SelectedIndex)
            {
                case 1:
                    result = result.OrderBy(p => p.Amount);
                    break;
                case 2:
                    result = result.OrderByDescending(p => p.Amount);
                    break;
            }
            
            ProductListView.ItemsSource = result.ToList();
        }

        private void LoadSuppliers()
        {
            using var db = new Dem2303Context();
            SupplierFilterBox.Items.Clear();
            SupplierFilterBox.Items.Add("Все поставщики");

            var suppliers = db.Suppliers.Select(s => s.Name);

            foreach (var supplier in suppliers)
            {
                SupplierFilterBox.Items.Add(supplier);
            }
        }

        private void ProductListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser != null &&
                _currentUser.Role.Name == "Администратор" &&
                ProductListView.SelectedItem is Product product) { 
                _frame.Navigate(new EditProductPage(_frame, product.ProductId));
            }
        }
    }
}
