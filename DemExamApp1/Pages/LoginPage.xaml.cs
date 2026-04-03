using DemExamApp1.Models;
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
using Microsoft.EntityFrameworkCore;

namespace DemExamApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private Frame _frame;
        public LoginPage(Frame mainFrame)
        {
            InitializeComponent();
            _frame = mainFrame;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new Dem2303Context();

            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => 
                       u.Login == LoginTextBox.Text && 
                       u.Password == PasswordBox.Password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _frame.Navigate(new ProductPage(_frame, user));
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new ProductPage(_frame, null));
        }
    }
}
