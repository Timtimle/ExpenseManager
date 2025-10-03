using System;
using System.Collections.Generic;
using ExpenseManager.Models;

namespace ExpenseManager.Services.Monthly {
    public interface IMonthlyExpenseService {
        bool AddExpense(decimal amount, string description, string category, DateTime date);
        List<Expense> GetExpensesByMonth(int year, int month);
        List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate);
        decimal GetMonthlyTotal(int year, int month);
        Dictionary<string, decimal> GetMonthlyCategoryTotals(int year, int month);
        List<MonthlyReport> GetYearlyReport(int year);
        bool ArchiveMonth(int year, int month);
    }
}