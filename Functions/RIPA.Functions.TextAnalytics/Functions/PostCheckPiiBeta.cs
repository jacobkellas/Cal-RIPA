using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RIPA.Functions.TextAnalytics.Services.TextAnalytics.Contracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RIPA.Functions.TextAnalytics.Functions
{
    public class PostCheckPiiBeta
    {
        private readonly IPiiTextAnalyticsService _piiTextAnalyticsService;
        public PostCheckPiiBeta(IPiiTextAnalyticsService piiTextAnalyticsService)
        {
            _piiTextAnalyticsService = piiTextAnalyticsService;
        }

        [FunctionName("PostCheckPiiBeta")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string document = data?.Document;

            if (string.IsNullOrEmpty(document))
            {
                return new BadRequestObjectResult("Must Provide Document");
            }

            var piiEntities = await _piiTextAnalyticsService.GetPiiEntities(document);
            PiiResponse piiResponse = new PiiResponse() { RedactedText = piiEntities.RedactedText, PiiEntities = new List<PiiEntity>() };

            foreach (var entity in piiEntities.Where(x => x.ConfidenceScore > .75))
            {
                piiResponse.PiiEntities.Add(new PiiEntity
                {
                    EntityText = entity.Text,
                    ConfidenceScore = $"{entity.ConfidenceScore:F2}",
                    Category = entity.Category.ToString()
                });
            }

            return new OkObjectResult(piiResponse);
        }

        public class PiiResponse
        {
            public List<PiiEntity> PiiEntities { get; set; }
            public string RedactedText { get; set; }
        }

        public class PiiEntity
        {
            public string EntityText { get; set; }
            public string ConfidenceScore { get; set; }
            public string Category { get; set; }
        }
    }
}
