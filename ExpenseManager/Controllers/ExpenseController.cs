using System;
using System.Collections.Generic;
using ExpenseManager.Models;
using ExpenseManager.Services;

namespace ExpenseManager.Controllers {
    public class ExpenseController {
        private ExpenseManager.Services.ExpenseManager expenseManager;

        public ExpenseController() {
            expenseManager = new ExpenseManager.Services.ExpenseManager();
        }

        public ExpenseController(ExpenseManager.Services.ExpenseManager expenseManagerService) {
            expenseManager = expenseManagerService;
        }

        public Services.ExpenseManager ExpenseManager {
            get => default;
            set {
            }
        }

        public bool AddExpense(decimal amount, string description, string category) {
            return expenseManager.AddExpense(amount, description, category);
        }

        public bool AddExpense(decimal amount, string description, string category, DateTime date) {
            return expenseManager.AddExpense(amount, description, category, date);
        }

        public List<Expense> GetAllExpenses() {
            return expenseManager.GetAllExpenses();
        }

        public decimal GetTotalExpenses() {
            return expenseManager.GetTotalExpenses();
        }

        public bool DeleteExpense(int index) {
            return expenseManager.DeleteExpense(index);
        }

        public bool LoadExpensesFromJson() {
            return expenseManager.LoadExpensesFromJson();
        }

        public List<Expense> GetExpensesByCategory(string category) {
            return expenseManager.GetExpensesByCategory(category);
        }

        public List<ExpenseData> GetExpensesAsTrainingData() {
            return expenseManager.GetExpensesAsTrainingData();
        }
    }
}