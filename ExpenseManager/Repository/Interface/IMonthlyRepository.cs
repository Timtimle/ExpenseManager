using System;
using System.Collections.Generic;
using ExpenseManager.Models;

namespace ExpenseManager.Repository.Interface {
    public interface IMonthlyRepository {
        bool SaveMonthlyData(int year, int month, List<Expense> expenses);
        List<Expense> LoadMonthlyData(int year, int month);
        bool ArchiveMonth(int year, int month);
        List<MonthlyReport> GetAvailableMonths();
        bool DeleteMonthlyData(int year, int month);
    }
}