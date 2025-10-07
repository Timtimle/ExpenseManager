using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Models;
using ExpenseManager.Services.Monthly;
using ExpenseManager.Repository;

namespace ExpenseManager.Controllers {
    public class MonthlyController {
        private readonly IMonthlyExpenseService monthlyService;

        public MonthlyController() {
            var repository = new ExpenseRepository();
            monthlyService = new MonthlyExpenseService(repository);
        }

        // Reports
        public MonthlyReport GetMonthlyReport(int year, int month) {
            var expenses = monthlyService.GetExpensesByMonth(year, month);
            var categoryTotals = monthlyService.GetMonthlyCategoryTotals(year, month);
            
            return new MonthlyReport(year, month) {
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalTransactions = expenses.Count,
                FoodTotal = categoryTotals["Food"],
                ShoppingTotal = categoryTotals["Shopping"],
                EntertainmentTotal = categoryTotals["Entertainment"],
                OthersTotal = categoryTotals["Others"],
                AveragePerDay = expenses.Count > 0 ? expenses.Sum(e => e.Amount) / DateTime.DaysInMonth(year, month) : 0
            };
        }

    }
}