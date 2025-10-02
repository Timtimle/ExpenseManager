using System.Collections.Generic;

namespace ExpenseManager.Models {
    public class ModelData {
        public List<string> Vocabulary { get; set; }
        public Dictionary<string, List<double>> Weights { get; set; }
        public Dictionary<string, double> Bias { get; set; }
        public int TrainingEpochs { get; set; }
        public string Version { get; set; }

        public ModelData() {
            Vocabulary = new List<string>();
            Weights = new Dictionary<string, List<double>>();
            Bias = new Dictionary<string, double>();
            Version = "1.0";
        }
    }
}