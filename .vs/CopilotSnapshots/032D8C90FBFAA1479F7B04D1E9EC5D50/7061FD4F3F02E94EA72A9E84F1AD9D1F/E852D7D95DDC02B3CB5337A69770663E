using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ExpenseManager.Models;
using ExpenseManager.Repository;
using ExpenseManager.Repository.Interface;

namespace ExpenseManager.Services {
    public class ExpenseManager {
        private IExpenseRepository repository;

        public ExpenseManager() {
            repository = new ExpenseRepository();
        }

        public ExpenseManager(IExpenseRepository expenseRepository) {
            repository = expenseRepository;
        }

        public bool AddExpense(decimal amount, string description, string category) {
            return AddExpense(amount, description, category, DateTime.Now);
        }

        public bool AddExpense(decimal amount, string description, string category, DateTime date) {
            System.Diagnostics.Debug.WriteLine($"🔍 ExpenseManager.AddExpense called: {amount} - {description} - {category} - {date:yyyy-MM-dd}");
            
            if (!ValidateExpense(amount, description, category)) {
                System.Diagnostics.Debug.WriteLine("❌ ExpenseManager: Validation failed");
                return false;
            }

            try {
                Expense expense = new Expense(amount, description, date, category);
                System.Diagnostics.Debug.WriteLine($"📝 ExpenseManager: Created expense object - {expense.Amount} - {expense.Description} - {expense.Date:yyyy-MM-dd} - {expense.Category}");
                
                bool result = repository.AddExpense(expense);
                System.Diagnostics.Debug.WriteLine($"💾 ExpenseManager: Repository.AddExpense result = {result}");
                
                if (result) {
                    var allExpenses = repository.GetAllExpenses();
                    System.Diagnostics.Debug.WriteLine($"📊 ExpenseManager: Repository now has {allExpenses.Count} expenses");
                }
                
                return result;
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"❌ ExpenseManager: Exception in AddExpense - {ex.Message}");
                return false;
            }
        }

        public List<Expense> GetAllExpenses() {
            return repository.GetAllExpenses();
        }

        public decimal GetTotalExpenses() {
            return repository.GetTotalAmount();
        }

        public bool DeleteExpense(int index) {
            return repository.DeleteExpense(index);
        }

        public List<Expense> GetExpensesByCategory(string category) {
            return repository.GetExpensesByCategory(category);
        }

        // Business logic methods
        public bool LoadExpensesFromJson() {
            try {
                string jsonFilePath = @"..\..\Data\User\expenses.json";
                
                if (!File.Exists(jsonFilePath)) {
                    return false;
                }

                string json = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
                
                JsonSerializerOptions options = new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                };
                
                List<Expense> loadedExpenses = JsonSerializer.Deserialize<List<Expense>>(json, options);
                
                if (loadedExpenses != null && loadedExpenses.Count > 0) {
                    // Clear existing data và load new data
                    List<Expense> currentExpenses = new List<Expense>();
                    
                    foreach (Expense expense in loadedExpenses) {
                        repository.AddExpense(expense);
                    }
                    
                    repository.SaveExpenses();
                    return true;
                }
                return false;
            } catch (Exception) {
                return false;
            }
        }

        // Convert methods for ML - sử dụng Expense model từ repository
        public List<ExpenseData> GetExpensesAsTrainingData() {
            List<ExpenseData> trainingData = new List<ExpenseData>();
            List<Expense> expenses = repository.GetAllExpenses();
            
            foreach (Expense expense in expenses) {
                trainingData.Add(new ExpenseData(expense.Category, expense.Description));
            }
            
            return trainingData;
        }

        private bool ValidateExpense(decimal amount, string description, string category) {
            if (amount <= 0) return false;
            if (string.IsNullOrWhiteSpace(description)) return false;
            if (string.IsNullOrWhiteSpace(category)) return false;
            return true;
        }
    }
}