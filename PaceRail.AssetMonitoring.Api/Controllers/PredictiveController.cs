using Microsoft.AspNetCore.Mvc;
using PaceInfrastructure.Services;

namespace PaceInfrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictiveController : ControllerBase
    {
        private readonly PredictiveMaintenanceService _predictiveService = new();

        [HttpGet("forecast/{assetId}")]
        public IActionResult GetForecast(int assetId)
        {
            // Simulated telemetry trend analysis
            var mockReadings = new List<double> { 45.2, 48.1, 52.6, 58.0, 64.3 };
            var result = _predictiveService.EvaluateAssetLifespan(assetId, mockReadings);
            return Ok(result);
        }
    }
}