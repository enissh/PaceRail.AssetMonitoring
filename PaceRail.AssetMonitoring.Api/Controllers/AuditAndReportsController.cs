using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace PaceInfrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditAndReportsController : ControllerBase
    {
        private static string _configuredWebhookUrl = "";
        private static readonly List<string> _webhookLogs = new();

        [HttpGet("export-audit-csv")]
        public IActionResult ExportAuditCsv()
        {
            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,EventType,AssetTag,Details,Status");
            csv.AppendLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss},SAFETY_SCAN,CIV-ECM1-202,Proximity hazard evaluation completed,RESOLVED");
            csv.AppendLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss},WORK_ORDER_DISPATCH,WO-8841,Urgent track realignment team assigned,IN_PROGRESS");

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"PACE_Audit_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        [HttpGet("export-pdf-report")]
        public IActionResult ExportPdfReport()
        {
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>PACE Compliance Audit Report</title>
                <style>
                    body {{ font-family: Arial, sans-serif; padding: 40px; color: #0f172a; }}
                    h1 {{ color: #1e3a8a; font-size: 22px; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 25px; }}
                    th, td {{ border: 1px solid #cbd5e1; padding: 10px; text-align: left; font-size: 13px; }}
                    th {{ background-color: #f1f5f9; }}
                </style>
            </head>
            <body>
                <h1>PACE Infrastructure - Official Regulatory Compliance Audit</h1>
                <p><strong>Generated UTC:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>
                <p><strong>Safety Standard:</strong> ISO-9001 / European Rail Traffic Management Directive</p>
                <table>
                    <tr><th>Event ID</th><th>Timestamp</th><th>Category</th><th>Asset Tag</th><th>Description</th><th>Compliance</th></tr>
                    <tr><td>EVT-9921</td><td>{DateTime.UtcNow.AddHours(-2):yyyy-MM-dd HH:mm:ss}</td><td>SAFETY_SCAN</td><td>CIV-ECM1-202</td><td>Spatial proximity clearance verified</td><td>COMPLIANT</td></tr>
                    <tr><td>EVT-9922</td><td>{DateTime.UtcNow.AddHours(-1):yyyy-MM-dd HH:mm:ss}</td><td>ANOMALY</td><td>CIV-ECM1-202</td><td>Thermal spike simulation logged (78.5°C)</td><td>REVIEWED</td></tr>
                </table>
                <script>window.print();</script>
            </body>
            </html>";
            return Content(html, "text/html");
        }

        [HttpPost("webhook-config")]
        public IActionResult ConfigureWebhook([FromBody] WebhookConfigDto dto)
        {
            _configuredWebhookUrl = dto.Url;
            _webhookLogs.Insert(0, $"[{DateTime.UtcNow:HH:mm:ss}] Webhook updated: {dto.Url}");
            return Ok(new { success = true, url = _configuredWebhookUrl });
        }

        [HttpGet("webhook-logs")]
        public IActionResult GetWebhookLogs()
        {
            return Ok(_webhookLogs);
        }
    }

    public class WebhookConfigDto
    {
        public string Url { get; set; } = string.Empty;
    }
}