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
        private string csvFilePath = "expenses.csv";
        private string jsonFilePath = @"..\..\Data\User\expenses.json";

        public ExpenseRepository() {
            expenses = new List<Expense>();
            LoadExpenses();
        }

        public bool AddExpense(Expense expense) {
            try {
                expenses.Insert(0, expense); // Add to top
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
                // ? Save to CSV (existing functionality)
                using (StreamWriter sw = new StreamWriter(csvFilePath)) {
                    sw.WriteLine("Amount,Description,Date,Category");
                    foreach (Expense expense in expenses) {
                        sw.WriteLine($"{expense.Amount},{expense.Description},{expense.Date},{expense.Category}");
                    }
                }

                // ? ALSO save to JSON for consistency
                try {
                    string directory = System.IO.Path.GetDirectoryName(jsonFilePath);
                    if (!System.IO.Directory.Exists(directory)) {
                        System.IO.Directory.CreateDirectory(directory);
                    }

                    var options = new System.Text.Json.JsonSerializerOptions { 
                        WriteIndented = true 
                    };
                    string json = System.Text.Json.JsonSerializer.Serialize(expenses, options);
                    System.IO.File.WriteAllText(jsonFilePath, json, new System.Text.UTF8Encoding(false));
                    
                    System.Diagnostics.Debug.WriteLine($"? Saved {expenses.Count} expenses to both CSV and JSON");
                } catch (Exception jsonEx) {
                    System.Diagnostics.Debug.WriteLine($"?? JSON save failed: {jsonEx.Message}");
                    // Don't fail the entire operation if JSON save fails
                }

                return true;
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"? SaveExpenses failed: {ex.Message}");
                return false;
            }
        }

        public bool LoadExpenses() {
            try {
                if (File.Exists(csvFilePath)) {
                    string[] lines = File.ReadAllLines(csvFilePath);
                    expenses.Clear();
                    
                    for (int i = 1; i < lines.Length; i++) { // Skip header
                        string[] parts = lines[i].Split(',');
                        if (parts.Length >= 4) {
                            decimal amount = decimal.Parse(parts[0]);
                            string description = parts[1];
                            DateTime date = DateTime.Parse(parts[2]);
                            string category = parts[3];
                            
                            expenses.Add(new Expense(amount, description, date, category));
                        }
                    }
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public decimal GetTotalAmount() {
            return expenses.Sum(e => e.Amount);
        }
    }
}