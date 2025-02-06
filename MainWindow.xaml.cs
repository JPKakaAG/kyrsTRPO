using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;
using Microsoft.Data.SqlClient;


namespace kyrsTRPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DataFilePath = @"C:\Users\devya\source\repos\JPKakaAG\kyrsTRPO\data\finance_data.json";
        private string currentUserLogin;
        private ResourceDictionary lightTheme;
        private ResourceDictionary darkTheme;
        public MainWindow(string userLogin)
        {
            InitializeComponent();
            currentUserLogin = userLogin;
            LoadFinanceData();
            Gmain.Visibility = Visibility.Visible;
            Gsettings.Visibility = Visibility.Collapsed;
        }
        private void LoadFinanceData()
        {
            if (File.Exists(DataFilePath))
            {
                string json = File.ReadAllText(DataFilePath);
                var records = JsonSerializer.Deserialize<List<FinanceRecord>>(json);

                // Фильтруем записи по логину текущего пользователя
                var userRecords = records.Where(r => r.Login == currentUserLogin).ToList();

                // Считаем общий баланс
                decimal totalIncome = userRecords.Sum(r => r.Income);
                decimal totalExpense = userRecords.Sum(r => r.Expense);
                decimal balance = totalIncome - totalExpense;

                // Обновляем текст блока с балансом
                BalanceTextBlock.Text = $"Баланс: {balance:C}"; // Выводим в формате валюты
            }
            else
            {
                BalanceTextBlock.Text = "Нет данных.";
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            decimal income;
            decimal expense;

            if (decimal.TryParse(IncomeTextBox.Text, out income) && decimal.TryParse(ExpenseTextBox.Text, out expense))
            {
                List<FinanceRecord> records;

                if (File.Exists(DataFilePath))
                {
                    // Чтение существующих записей
                    string json = File.ReadAllText(DataFilePath);
                    records = JsonSerializer.Deserialize<List<FinanceRecord>>(json) ?? new List<FinanceRecord>();
                }
                else
                {
                    records = new List<FinanceRecord>();
                }

                // Найти запись для текущего пользователя
                var existingRecord = records.FirstOrDefault(r => r.Login == currentUserLogin);

                if (existingRecord != null)
                {
                    // Обновить существующую запись
                    existingRecord.Income += income; // Добавляем новый доход
                    existingRecord.Expense += expense; // Добавляем новый расход
                }
                else
                {
                    // Создать новую запись если её нет
                    var newRecord = new FinanceRecord
                    {
                        Login = currentUserLogin,
                        Income = income,
                        Expense = expense
                    };

                    records.Add(newRecord);
                }

                // Сохранение обновленных данных в файл
                string updatedJson = JsonSerializer.Serialize(records);
                File.WriteAllText(DataFilePath, updatedJson);

                // После добавления перезагружаем данные
                LoadFinanceData();
            }
            else
            {
                MessageBox.Show("Введите корректные значения дохода и расхода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем поля ввода
            IncomeTextBox.Text = string.Empty;
            ExpenseTextBox.Text = string.Empty;

            // Создаем новую запись с нулевыми значениями
            var resetRecord = new FinanceRecord
            {
                Login = currentUserLogin,
                Income = 0,
                Expense = 0
            };

            List<FinanceRecord> records;

            if (File.Exists(DataFilePath))
            {
                // Читаем существующие записи
                string json = File.ReadAllText(DataFilePath);
                records = JsonSerializer.Deserialize<List<FinanceRecord>>(json) ?? new List<FinanceRecord>();

                // Удаляем все записи текущего пользователя и добавляем обновлённую запись
                records.RemoveAll(r => r.Login == currentUserLogin);
            }
            else
            {
                records = new List<FinanceRecord>();
            }

            // Добавляем новую запись с нулевыми значениями
            records.Add(resetRecord);

            // Сохранение обновленных данных в файл
            string updatedJson = JsonSerializer.Serialize(records);
            File.WriteAllText(DataFilePath, updatedJson);

            // После сброса перезагружаем данные
            LoadFinanceData();
        }

        private void btn_settingsClick(object sender, RoutedEventArgs e)
        {
            Gmain.Visibility = Visibility.Collapsed;
            Gsettings.Visibility = Visibility.Visible;
        }

        private void btn_backClick(object sender, RoutedEventArgs e)
        {
            Gsettings.Visibility = Visibility.Collapsed;    
            Gmain.Visibility = Visibility.Visible;
        }

        private void DelAc_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить свою учетную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Удаляем учетную запись из базы данных
                DeleteUserAccount(currentUserLogin);

                // Перенаправляем на окно авторизации
                var loginWindow = new Avt(); // создаем новое окно авторизации
                loginWindow.Show(); // отображаем его
                this.Close(); // закрываем текущее окно
            }
        }
        private void DeleteUserAccount(string login)
        {
            string connectionString = "Server=DESKTOP-I00R4RJ; Database=finance; User=des; Password=1234567890; Encrypt=false";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Пользователь WHERE Логин = @login", connection);
                command.Parameters.AddWithValue("@login", login);

                command.ExecuteNonQuery();
            }
        }
        private void DarkTheme_Checked(object sender, RoutedEventArgs e)
        {
            ApplyTheme("DarkThemeStyle");
            this.Background = Brushes.Gray;
            
        }
        private void DarkTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyTheme("LightThemeStyle");
            this.Background = Brushes.White;
        }
        private void ApplyTheme(string themeKey)
        {
            // Применяем стиль ко всем элементам в Gmain
            foreach (var child in Gmain.Children)
            {
                if (child is Control control)
                {
                    control.Style = (Style)FindResource(themeKey);
                }
            }

            // Применяем стиль ко всем элементам в Gsettings
            foreach (var child in Gsettings.Children)
            {
                if (child is Control control)
                {
                    control.Style = (Style)FindResource(themeKey);
                }
            }
        }

    }
}