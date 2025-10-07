using System.Collections.Generic;
using ExpenseManager.Services.Prediction;

namespace ExpenseManager.Services.Prediction {
    public class ExpensePredictor {
        private List<IExpenseClassifier> classifiers;
        private MLClassifier mlClassifier;
        private RuleBasedClassifier ruleBasedClassifier;

        public ExpensePredictor(List<IExpenseClassifier> classifiers) {
            this.classifiers = classifiers;
            
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

            // Try rule-based classification first (faster)
            if (ruleBasedClassifier != null) {
                string ruleResult = ruleBasedClassifier.Classify(description);
                if (ruleResult != "Khac") {
                    return ruleResult;
                }
            }

            // Fall back to ML classification
            if (mlClassifier != null) {
                return mlClassifier.Classify(description);
            }

            return "Khac";
        }
    }
}