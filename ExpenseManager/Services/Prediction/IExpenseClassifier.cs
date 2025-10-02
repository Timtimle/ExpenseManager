namespace ExpenseManager.Services.Prediction {
    public interface IExpenseClassifier {
        string Classify(string description);
    }
}