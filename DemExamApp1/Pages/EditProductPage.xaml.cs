using DemExamApp1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace DemExamApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditProductPage.xaml
    /// </summary>
    public partial class EditProductPage : Page
    {
        private Frame _frame;
        private Product _product;
        private string _newImagePath;
        private Dem2303Context db = new Dem2303Context();

        public EditProductPage(Frame frame, int productId)
        {
            InitializeComponent();
            _frame = frame;
          
            LoadLists();

            if(productId != 0)
            {
                LoadProduct(productId);
                Title = "Редактирование товара";
            }
            else
            {
                _product = new Product();
                Title = "Добавление товара";
            }
        }

        private void LoadProduct(int productId)
        {
            _product = db.Products.Include(p => p.Category).First(p => p.ProductId == productId);

            ArticleBox.Text = _product.Arcticle;
            NameBox.Text = _product.Name;
            DescriptionBox.Text = _product.Discription;
            PriceBox.Text = _product.Price.ToString();
            DiscountBox.Text = _product.Discount.ToString();
            AmountBox.Text = _product.Amount.ToString();
            UnitBox.Text = _product.Unit;

            CategoryBox.SelectedIndex = _product.CategoryId;
            ManufacturerBox.SelectedIndex = _product.CategoryId;
            SupplierBox.SelectedIndex = _product.CategoryId;

            ProductImage.Source = new BitmapImage(new Uri(_product.PhotoPath));
        }

        private void LoadLists()
        {
            using var db = new Dem2303Context();

            CategoryBox.ItemsSource = db.Categories.ToList();
            ManufacturerBox.ItemsSource = db.Manufacturers.ToList();
            SupplierBox.ItemsSource = db.Suppliers.ToList();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _product.Arcticle = ArticleBox.Text;
            _product.Name = NameBox.Text;
            _product.Discription = DescriptionBox.Text;
            _product.Price = decimal.Parse(PriceBox.Text);
            if(_product.Price <0)
            {
                return;
            }
            _product.Unit = UnitBox.Text;
            _product.Amount = int.Parse(AmountBox.Text);
            if(_product.Amount < 0)
            {
                return;
            }
            _product.Discount = int.Parse(DiscountBox.Text);
            if(_product.Discount < 0 || _product.Discount > 100)
            {
                return;
            }

            if (SupplierBox.SelectedItem is Supplier selectedSupplier)
            {
                _product.SupplierId = selectedSupplier.SupplierId; 
            }

            if (ManufacturerBox.SelectedItem is Manufacturer selectedManuf)
            {
                _product.ManufacturerId = selectedManuf.ManufacturerId;
            }

            if (CategoryBox.SelectedItem is Category selectedCategory)
            {
                _product.CategoryId = selectedCategory.CategoryId;
            }

            _product.Supplier = null;
            _product.Manufacturer = null;
            _product.Category = null;

            if (!String.IsNullOrEmpty(_newImagePath))
            {
                _product.Photo = Path.GetFileName(_newImagePath);
            }

            if (_product.ProductId == 0)
            {
                db.Products.Add(_product); 
            }
            else
            {
                db.Products.Update(_product); 
            }
            db.SaveChanges();

            _frame.GoBack();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            bool isOrdered = db.OrderedProducts.Any(p=>p.ProductId == _product.ProductId);

            if (isOrdered)
            {
                return;
            }

            db.Products.Remove(_product);
            db.SaveChanges();
            _frame.GoBack();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (dialog.ShowDialog() != true)
                return;

            var image = new BitmapImage(new Uri(dialog.FileName));

            if (image.PixelWidth > 300 || image.PixelHeight > 200)
            {
                MessageBox.Show("Выберите изображение размером не более 300x200 пикселей",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var folder = Path.Combine(Environment.CurrentDirectory, "Images");
            Directory.CreateDirectory(folder);

            _newImagePath = Path.Combine(folder, Path.GetFileName(dialog.FileName));
            File.Copy(dialog.FileName, _newImagePath, true);
            ProductImage.Source = image;
        }
    }
}
