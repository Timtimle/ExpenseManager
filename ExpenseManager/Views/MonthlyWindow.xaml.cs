using System;
using System.Windows;
using System.Windows.Controls;
using ExpenseManager.Controllers;

namespace ExpenseManager.Views {
    public partial class MonthlyWindow : Window {
        private MonthlyController monthlyController;

        public MonthlyWindow() {
            InitializeComponent();
            monthlyController = new MonthlyController();
            InitializeForm();
        }

        public MonthlyController MonthlyController {
            get => default;
            set {
            }
        }

        private void InitializeForm() {
            // Set current month/year as default
            var now = DateTime.Now;
            MonthComboBox.SelectedIndex = now.Month - 1;
            
            // Populate year dropdown (last 10 year)
            for (int i = now.Year; i >= now.Year - 10; i--) {
                YearComboBox.Items.Add(i);
            }
            YearComboBox.SelectedIndex = 0;
            
            // Setup event handlers
            LoadMonthButton.Click += LoadMonthButton_Click;
            
            // Load current month by default
            LoadCurrentMonth();
        }

        private void LoadMonthButton_Click(object sender, RoutedEventArgs e) {
            LoadSelectedMonth();
        }

        private void LoadCurrentMonth() {
            var now = DateTime.Now;
            LoadMonthData(now.Year, now.Month);
        }

        private void LoadSelectedMonth() {
            try {
                var selectedYear = (int)YearComboBox.SelectedItem;
                var selectedMonth = ((ComboBoxItem)MonthComboBox.SelectedItem).Tag;
                var month = int.Parse(selectedMonth.ToString());
                
                LoadMonthData(selectedYear, month);
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMonthData(int year, int month) {
            try {
                // Get monthly report
                var report = monthlyController.GetMonthlyReport(year, month);
                
                // Update summary stats
                TotalAmountLabel.Text = $"{report.TotalAmount:N0} VND";
                TotalTransactionsLabel.Text = report.TotalTransactions.ToString();
                AveragePerDayLabel.Text = $"{report.AveragePerDay:N0} VND";
                
                // Update category breakdown
                FoodLabel.Text = $"Food: {report.FoodTotal:N0} VND";
                ShoppingLabel.Text = $"Shopping: {report.ShoppingTotal:N0} VND";
                EntertainmentLabel.Text = $"Entertainment: {report.EntertainmentTotal:N0} VND";
                OthersLabel.Text = $"Others: {report.OthersTotal:N0} VND";
                
                // Update window title
                this.Title = $"Monthly Report - {report.MonthName}";
                
            } catch (Exception ex) {
                MessageBox.Show($"Error loading monthly data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}