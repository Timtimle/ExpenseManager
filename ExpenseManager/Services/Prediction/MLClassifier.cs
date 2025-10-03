using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ExpenseManager.Services.Prediction;
using ExpenseManager.Models;
using ExpenseManager.Utils;

namespace ExpenseManager.Services.Prediction {
    public class MLClassifier : IExpenseClassifier {
        private List<ExpenseData> trainingData = new List<ExpenseData>();
        private List<string> vocabulary = new List<string>();
        private Dictionary<string, List<double>> weights;
        private Dictionary<string, double> bias;

        public List<string> getVocabulary() => vocabulary;
        public List<ExpenseData> getTrainingData() => trainingData;

        public List<string> setVocabulary(List<string> vocab) {
            this.vocabulary = vocab;
            return vocabulary;
        }

        public List<ExpenseData> LoadFromCsv(string filePath) {
            List<ExpenseData> data = new List<ExpenseData>();

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; ++i) {
                string line = lines[i];
                string[] parts = line.Split(',');

                if (parts.Length >= 2) {
                    data.Add(new ExpenseData(parts[0].Trim(), parts[1].Trim()));
                }
            }
            return data;
        }

        public void LoadTrainingData() {
            Vietnamese vietnamese = new Vietnamese();

            List<ExpenseData> data1 = this.LoadFromCsv(@"..\..\Data\TrainingData\anuong.csv").Select(des => new ExpenseData(des.Label, vietnamese.RemoveDiacritics(des.Text))).ToList();
            List<ExpenseData> data2 = this.LoadFromCsv(@"..\..\Data\TrainingData\muasam.csv").Select(des => new ExpenseData(des.Label, vietnamese.RemoveDiacritics(des.Text))).ToList();
            List<ExpenseData> data3 = this.LoadFromCsv(@"..\..\Data\TrainingData\giaitri.csv").Select(des => new ExpenseData(des.Label, vietnamese.RemoveDiacritics(des.Text))).ToList();
            List<ExpenseData> data4 = this.LoadFromCsv(@"..\..\Data\TrainingData\khac.csv").Select(des => new ExpenseData(des.Label, vietnamese.RemoveDiacritics(des.Text))).ToList();

            // DBG dbg = new DBG();
            // dbg.Out(data1);

            trainingData.AddRange(data1);
            trainingData.AddRange(data2);
            trainingData.AddRange(data3);
            trainingData.AddRange(data4);
        }

        public void Train(List<ExpenseData> data, int epochs, bool isTrained) {
            if (!isTrained)
                return;

            trainingData = data;
            weights = new Dictionary<string, List<double>>();
            bias = new Dictionary<string, double>();

            foreach (var val in data) {
                string[] words = val.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words) {
                    string w = word.ToLower();
                    if (!vocabulary.Contains(w)) {
                        vocabulary.Add(w);
                    }
                }
            }

            List<List<int>> x = new List<List<int>>();
            List<string> y = new List<string>();

            foreach (var val in trainingData) {
                List<int> tmp = new List<int>();
                string[] words = val.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var vocaWord in vocabulary) {
                    tmp.Add(words.Contains(vocaWord) ? 1 : 0);
                }

                x.Add(tmp);
                y.Add(val.Label);
            }

            Random rand = new Random(42);
            List<string> categories = y.Distinct().ToList();

            foreach (var category in categories) {
                weights[category] = new List<double>();

                for (int i = 0; i < vocabulary.Count; ++i) {
                    double randomWeight = (rand.NextDouble() - 0.5) * 0.01;
                    weights[category].Add(randomWeight);
                }
                bias[category] = 0.0;
            }

            for (int epoch = 0; epoch < epochs; ++epoch) {
                List<Dictionary<string, double>> allZ = new List<Dictionary<string, double>>();

                foreach (var xi in x) {
                    Dictionary<string, double> z = new Dictionary<string, double>();

                    foreach (var category in categories) {
                        double sum = bias[category];
                        for (int i = 0; i < vocabulary.Count; ++i) {
                            sum += weights[category][i] * xi[i];
                        }
                        z[category] = sum;
                    }
                    allZ.Add(z);
                }

                List<Dictionary<string, double>> allSoftMax = new List<Dictionary<string, double>>();

                foreach (var z in allZ) {
                    double maxZ = double.MinValue;
                    foreach (var val in z.Values) {
                        maxZ = Math.Max(maxZ, val);
                    } // find max to avoid overflow

                    Dictionary<string, double> expZ = new Dictionary<string, double>();
                    double sumExpZ = 0.0;
                    foreach (var val in z) {
                        double e = Math.Exp(val.Value - maxZ);
                        expZ[val.Key] = e;
                        sumExpZ += e;
                    }

                    Dictionary<string, double> softMax = new Dictionary<string, double>();
                    foreach (var val in expZ) {
                        softMax[val.Key] = val.Value / sumExpZ;
                    }

                    allSoftMax.Add(softMax);
                }

                double totalLoss = 0.0;
                for (int i = 0; i < allSoftMax.Count; ++i) {
                    Dictionary<string, double> softMax = allSoftMax[i];
                    string trueLabel = y[i];
                    double epsilon = 1e-15;
                    double predictedProb = Math.Max(softMax[trueLabel], epsilon); // probability assigned to the true class
                    double loss = -Math.Log(predictedProb);
                    totalLoss += loss;
                }
                double avgLoss = totalLoss / allSoftMax.Count; // cross-entropy loss


                double learningRate = 0.01;
                for (int j = 0; j < x.Count; ++j) {
                    List<int> xi = x[j];
                    string trueLabel = y[j];
                    Dictionary<string, double> softMax = allSoftMax[j];

                    foreach (var category in categories) {
                        double y_c = (trueLabel == category) ? 1.0 : 0.0;
                        for (int i = 0; i < vocabulary.Count; ++i) {
                            double gradient_w = (softMax[category] - y_c) * xi[i];
                            weights[category][i] = weights[category][i] - learningRate * gradient_w;
                        }
                        double gradient_b = softMax[category] - y_c;
                        bias[category] = bias[category] - learningRate * gradient_b;
                    }
                } // Gradient Descent update weights & bias 
            }
        }
        public string Classify(string description) {
            string filePath = @"..\..\Data\Models\trained_model.json";
            if (!File.Exists(filePath)) {
                MLClassifier mLClassifier = new MLClassifier();
                mLClassifier.LoadTrainingData();
                mLClassifier.Train(mLClassifier.getTrainingData(), 1000, true);
                mLClassifier.SaveModel(filePath);
            }

            if (weights == null || weights.Count == 0) {
                return "Khac"; // Return default if model not trained
            }

            string[] words = description.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> x = new List<int>();
            foreach (var vocaWord in vocabulary) {
                x.Add(words.Contains(vocaWord) ? 1 : 0);
            }

            Dictionary<string, double> z = new Dictionary<string, double>();
            foreach (var category in weights.Keys) {
                double sum = bias[category];
                for (int i = 0; i < vocabulary.Count; ++i) {
                    sum += weights[category][i] * x[i];
                }
                z[category] = sum;
            }

            double maxZ = z.Values.Max();
            Dictionary<string, double> expZ = z.ToDictionary(kv => kv.Key, kv => Math.Exp(kv.Value - maxZ));
            double sumExpZ = expZ.Values.Sum();
            Dictionary<string, double> softMax = expZ.ToDictionary(kv => kv.Key, kv => kv.Value / sumExpZ);

            string predictedCategory = null;
            double maxProb = double.MinValue;
            foreach (var val in softMax) {
                if (val.Value > maxProb) {
                    maxProb = val.Value;
                    predictedCategory = val.Key;
                }
            }

            return predictedCategory ?? "Khac";
        }

        public void SaveModel(string filePath) {
            ModelData model = new ModelData
            {
                Vocabulary = vocabulary,
                Weights = weights,
                Bias = bias
            };

            File.WriteAllText(filePath, JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));
        }

        public void LoadModel(string filePath) {
            if (!File.Exists(filePath)) return;

            ModelData modelData = JsonSerializer.Deserialize<ModelData>(File.ReadAllText(filePath));

            vocabulary = modelData.Vocabulary;
            weights = modelData.Weights;
            bias = modelData.Bias;
        }

        public double EvaluateAccuracy(List<ExpenseData> testData) {
            int correct = 0;

            Vietnamese vietnamese = new Vietnamese();

            foreach (var item in testData) {
                string input = vietnamese.RemoveDiacritics(item.Text.ToLower());
                string predicted = Classify(input);

                string predictedNorm = vietnamese.RemoveDiacritics(predicted).ToLower();
                string trueLabelNorm = vietnamese.RemoveDiacritics(item.Label).ToLower();

                if (predictedNorm == trueLabelNorm)
                    correct++;
                using (StreamWriter sw = new StreamWriter("accurtest.txt", true)) {
                    sw.WriteLine($"True: {trueLabelNorm} - Predicted: {predictedNorm}" + " " + testData.Count + " " + correct + " " + (double)correct / testData.Count);
                }
            }

            return (double)correct / testData.Count;
        }
    }
}