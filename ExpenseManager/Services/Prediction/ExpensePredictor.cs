using System.Collections.Generic;
using ExpenseManager.Models;
using ExpenseManager.Services.Prediction;

namespace ExpenseManager.Services.Prediction {
    public class ExpensePredictor {
        private List<IExpenseClassifier> classifiers;
        private MLClassifier mlClassifier;
        private RuleBasedClassifier ruleBasedClassifier;

        public ExpensePredictor(List<IExpenseClassifier> classifiers) {
            foreach (IExpenseClassifier classifier in classifiers) {
                if (classifier is MLClassifier) {
                    mlClassifier = classifier as MLClassifier;       
                }
                else if (classifier is RuleBasedClassifier) {
                    ruleBasedClassifier = classifier as RuleBasedClassifier; 
                }
            }
        }

        public string Predict(string description) {
            if (string.IsNullOrWhiteSpace(description)) {
                return "Khac";
            }

            if (ruleBasedClassifier != null) {
                string ruleResult = ruleBasedClassifier.Classify(description);
                if (ruleResult != "Khac") {
                    return ruleResult;
                }
            }

            if (mlClassifier != null) {
                return mlClassifier.Classify(description);
            }

            return "Khac";
        }
    }
}