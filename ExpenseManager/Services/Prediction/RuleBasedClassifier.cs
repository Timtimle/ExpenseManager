using System.Collections.Generic;
using System.Linq;
using ExpenseManager.Services.Prediction;
using ExpenseManager.Utils;

namespace ExpenseManager.Services.Prediction {
    public class RuleBasedClassifier : IExpenseClassifier {
        public Dictionary<string, string[]> rules = new Dictionary<string, string[]> {
            {"Ăn Uống", new string[] {"an", "uong", "com", "ca phe", "tra", "nuoc"} },
            {"Mua Sắm", new string[] {"mua", "giay", "quan", "ao", "shopee", "tiki", "lazada", "tiktok"} },
            {"Giải Trí", new string[] {"xem", "phim", "game", "hat", "nhac"} },
        };

        public string Classify(string description) {
            description = description.ToLower();

            foreach (var rule in rules) {
                foreach (var keyword in rule.Value) {
                    if (description.Contains(keyword)) {
                        return rule.Key;
                    }
                }
            }
            return "Khac";
        }
    }
}