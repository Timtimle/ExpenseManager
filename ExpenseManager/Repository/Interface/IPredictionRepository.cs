using System.Collections.Generic;
using ExpenseManager.Models;

namespace ExpenseManager.Repository.Interface {
    public interface IPredictionRepository {
        bool SaveModel(ModelData modelData, string filePath);
        ModelData LoadModel(string filePath);
        bool ModelExists(string filePath);
        List<ExpenseData> LoadTrainingData();
        bool SaveTrainingInfo(int trainingCount, int epochs, string version);
        string GetModelInfo();
    }
}