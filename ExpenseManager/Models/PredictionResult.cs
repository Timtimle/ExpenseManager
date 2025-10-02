using System;

namespace ExpenseManager.Models {
    public class PredictionResult {
        public string PredictedCategory { get; set; }
        public double Confidence { get; set; }
        public string Method { get; set; }
        public DateTime PredictionTime { get; set; }

        public PredictionResult() {
            PredictionTime = DateTime.Now;
        }

        public PredictionResult(string predictedCategory, double confidence, string method) {
            PredictedCategory = predictedCategory;
            Confidence = confidence;
            Method = method;
            PredictionTime = DateTime.Now;
        }
    }
}