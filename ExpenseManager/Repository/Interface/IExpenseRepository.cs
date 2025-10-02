using System.Collections.Generic;
using ExpenseManager.Models;

namespace ExpenseManager.Repository.Interface {
    public interface IExpenseRepository {
        bool AddExpense(Expense expense);
        List<Expense> GetAllExpenses();
        List<Expense> GetExpensesByCategory(string category);
        bool DeleteExpense(int index);
        bool SaveExpenses();
        bool LoadExpenses();
        decimal GetTotalAmount();
    }
}