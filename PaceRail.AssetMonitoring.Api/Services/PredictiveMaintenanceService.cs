namespace PaceInfrastructure.Services
{
    public class PredictiveMaintenanceService
    {
        public PredictiveAnalysisResult EvaluateAssetLifespan(int assetId, List<double> recentReadings)
        {
            if (recentReadings == null || recentReadings.Count == 0)
            {
                return new PredictiveAnalysisResult { RemainingHours = 720, RiskLevel = "Low" };
            }

            double average = recentReadings.Average();
            double trendFactor = recentReadings.Last() - recentReadings.First();

            int estimatedHours = trendFactor > 5 ? 120 : (average > 70 ? 240 : 850);
            string risk = estimatedHours < 200 ? "High" : (estimatedHours < 500 ? "Moderate" : "Low");

            return new PredictiveAnalysisResult
            {
                AssetId = assetId,
                AverageMetric = Math.Round(average, 2),
                RemainingHours = estimatedHours,
                RiskLevel = risk
            };
        }
    }

    public class PredictiveAnalysisResult
    {
        public int AssetId { get; set; }
        public double AverageMetric { get; set; }
        public int RemainingHours { get; set; }
        public string RiskLevel { get; set; } = "Low";
    }
}