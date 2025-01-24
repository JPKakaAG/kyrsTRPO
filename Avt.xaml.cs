using kyrsTRPO.models;
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
using System.Windows.Shapes;

namespace kyrsTRPO
{
    /// <summary>
    /// Логика взаимодействия для Avt.xaml
    /// </summary>
    public partial class Avt : Window
    {
        private readonly FinanceContext _context;
        public Avt()
        {
            InitializeComponent();
            _context = new FinanceContext();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var user = _context.Пользовательs.FirstOrDefault(u => u.Логин == login && u.Пароль == password);

            if (user != null)
            {
                string userLogin = LoginTextBox.Text;
                MainWindow mainWindow = new MainWindow(userLogin); 
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var login = RegLoginTextBox.Text;
            var password = RegPasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingUser = _context.Пользовательs.Any(u => u.Логин == login);
            if (existingUser)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new Пользователь { Логин = login, Пароль = password };
            _context.Пользовательs.Add(newUser);
            _context.SaveChanges();
            MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            SwitchToLogin_Click(sender, e); // Переключаемся обратно на экран входа
        }
        private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
        }
        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            RegisterPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
        }
    }
}
