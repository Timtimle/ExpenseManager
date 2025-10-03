using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ExpenseManager.Models;
using ExpenseManager.Repository.Interface;

namespace ExpenseManager.Repository {
    public class ExpenseRepository : IExpenseRepository {
        private List<Expense> expenses;
        private string jsonFilePath = @"..\..\Data\User\expenses.json";

        public ExpenseRepository() {
            expenses = new List<Expense>();
            LoadExpenses();
        }

        public bool AddExpense(Expense expense) {
            try {
                expenses.Insert(0, expense);
                return SaveExpenses();
            } catch (Exception) {
                return false;
            }
        }

        public List<Expense> GetAllExpenses() {
            return expenses;
        }

        public List<Expense> GetExpensesByCategory(string category) {
            return expenses.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public bool DeleteExpense(int index) {
            try {
                if (index >= 0 && index < expenses.Count) {
                    expenses.RemoveAt(index);
                    return SaveExpenses();
                }
                return false;
            } catch (Exception) {
                return false;
            }
        }

        public bool SaveExpenses() {
            try {
                string directory = Path.GetDirectoryName(jsonFilePath);
                if (!Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(expenses, options);
                File.WriteAllText(jsonFilePath, json, System.Text.Encoding.UTF8);
                
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool LoadExpenses() {
            try {
                if (File.Exists(jsonFilePath)) {
                    string json = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    expenses = JsonSerializer.Deserialize<List<Expense>>(json, options) ?? new List<Expense>();
                } else {
                    expenses = new List<Expense>();
                }
                return true;
            } catch (Exception) {
                expenses = new List<Expense>();
                return false;
            }
        }

        public decimal GetTotalAmount() {
            return expenses.Sum(e => e.Amount);
        }
    }
}