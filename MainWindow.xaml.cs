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


namespace kyrsTRPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DataFilePath = @"C:\Users\isp41\source\repos\JPKakaAG\kyrsTRPO\data\finance_data.json";
        private string currentUserLogin;
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
                var newRecord = new FinanceRecord
                {
                    Login = currentUserLogin,
                    Income = income,
                    Expense = expense
                };
                List<FinanceRecord> records;
                if (File.Exists(DataFilePath))
                {
                    string json = File.ReadAllText(DataFilePath);
                    records = JsonSerializer.Deserialize<List<FinanceRecord>>(json) ?? new List<FinanceRecord>();
                }
                else
                {
                    records = new List<FinanceRecord>();
                }
                records.Add(newRecord);
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
    }
}