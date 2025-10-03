using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Models;
using ExpenseManager.Repository.Interface;

namespace ExpenseManager.Services.Monthly {
    public class MonthlyExpenseService : IMonthlyExpenseService {
        private readonly IExpenseRepository repository;

        public MonthlyExpenseService(IExpenseRepository repository) {
            this.repository = repository;
        }

        public bool AddExpense(decimal amount, string description, string category, DateTime date) {
            try {
                var expense = new Expense(amount, description, date, category);
                return repository.AddExpense(expense);
            } catch (Exception) {
                return false;
            }
        }

        public List<Expense> GetExpensesByMonth(int year, int month) {
            var allExpenses = repository.GetAllExpenses();
            return allExpenses.Where(e => e.Date.Year == year && e.Date.Month == month).ToList();
        }

        public List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate) {
            var allExpenses = repository.GetAllExpenses();
            return allExpenses.Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date).ToList();
        }

        public decimal GetMonthlyTotal(int year, int month) {
            var monthlyExpenses = GetExpensesByMonth(year, month);
            return monthlyExpenses.Sum(e => e.Amount);
        }

        public Dictionary<string, decimal> GetMonthlyCategoryTotals(int year, int month) {
            var monthlyExpenses = GetExpensesByMonth(year, month);
            return new Dictionary<string, decimal> {
                ["Food"] = monthlyExpenses.Where(e => IsFoodCategory(e.Category)).Sum(e => e.Amount),
                ["Shopping"] = monthlyExpenses.Where(e => IsShoppingCategory(e.Category)).Sum(e => e.Amount),
                ["Entertainment"] = monthlyExpenses.Where(e => IsEntertainmentCategory(e.Category)).Sum(e => e.Amount),
                ["Others"] = monthlyExpenses.Where(e => IsOthersCategory(e.Category)).Sum(e => e.Amount)
            };
        }

        public List<MonthlyReport> GetYearlyReport(int year) {
            var reports = new List<MonthlyReport>();
            
            for (int month = 1; month <= 12; month++) {
                var monthlyExpenses = GetExpensesByMonth(year, month);
                var categoryTotals = GetMonthlyCategoryTotals(year, month);
                
                var report = new MonthlyReport(year, month) {
                    TotalAmount = monthlyExpenses.Sum(e => e.Amount),
                    TotalTransactions = monthlyExpenses.Count,
                    FoodTotal = categoryTotals["Food"],
                    ShoppingTotal = categoryTotals["Shopping"],
                    EntertainmentTotal = categoryTotals["Entertainment"],
                    OthersTotal = categoryTotals["Others"],
                    AveragePerDay = monthlyExpenses.Count > 0 ? monthlyExpenses.Sum(e => e.Amount) / DateTime.DaysInMonth(year, month) : 0
                };
                
                reports.Add(report);
            }
            
            return reports;
        }

        public bool ArchiveMonth(int year, int month) {
            try {
                var monthlyExpenses = GetExpensesByMonth(year, month);
                // Logic ?? archive monthly data vào file riêng
                var archivePath = $@"..\..\Data\Archive\{year}\{month:00}-{year}-expenses.json";
                
                // Create directory if needed
                var directory = System.IO.Path.GetDirectoryName(archivePath);
                if (!System.IO.Directory.Exists(directory)) {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                // Save monthly data
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var json = System.Text.Json.JsonSerializer.Serialize(monthlyExpenses, options);
                System.IO.File.WriteAllText(archivePath, json);
                
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private bool IsFoodCategory(string category) {
            return category.ToLower().Contains("food") || category.Contains("???");
        }

        private bool IsShoppingCategory(string category) {
            return category.ToLower().Contains("shopping") || category.Contains("???");
        }

        private bool IsEntertainmentCategory(string category) {
            return category.ToLower().Contains("entertainment") || category.Contains("??");
        }

        private bool IsOthersCategory(string category) {
            return !IsFoodCategory(category) && !IsShoppingCategory(category) && !IsEntertainmentCategory(category);
        }
    }
}