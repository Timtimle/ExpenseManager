using System;

namespace ExpenseManager.Models {
    public class MonthlyReport {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalTransactions { get; set; }
        public decimal FoodTotal { get; set; }
        public decimal ShoppingTotal { get; set; }
        public decimal EntertainmentTotal { get; set; }
        public decimal OthersTotal { get; set; }
        public decimal AveragePerDay { get; set; }
        public DateTime GeneratedDate { get; set; }

        public MonthlyReport() {
            GeneratedDate = DateTime.Now;
        }

        public MonthlyReport(int year, int month) {
            Year = year;
            Month = month;
            MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy");
            GeneratedDate = DateTime.Now;
        }
    }
}