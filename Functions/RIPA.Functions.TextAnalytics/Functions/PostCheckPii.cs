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
    public class PostCheckPii
    {
        private readonly IPiiTextAnalyticsService _piiTextAnalyticsService;
        public PostCheckPii(IPiiTextAnalyticsService piiTextAnalyticsService)
        {
            _piiTextAnalyticsService = piiTextAnalyticsService;
        }

        [FunctionName("PostCheckPii")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string document = data?.Document;

            if (string.IsNullOrEmpty(document))
            {
                return new BadRequestObjectResult("Must Provide Document");
            }

            var categorizedEntities = await _piiTextAnalyticsService.GetCategorizedEntities(document);
            EntityResponse entityResponse = new EntityResponse() { Entities = new List<Entity>() };

            foreach (var entity in categorizedEntities.Where(x => x.ConfidenceScore > .75))
            {
                entityResponse.Entities.Add(new Entity
                {
                    EntityText = entity.Text,
                    ConfidenceScore = $"{entity.ConfidenceScore:F2}",
                    Category = entity.Category.ToString()
                });
            }

            entityResponse.RedactedText = RedactText(entityResponse.Entities, document);

            return new OkObjectResult(entityResponse);
        }

        public class EntityResponse
        {
            public List<Entity> Entities { get; set; }
            public string RedactedText { get; set; }
        }


        public class Entity
        {
            public string EntityText { get; set; }
            public string ConfidenceScore { get; set; }
            public string Category { get; set; }
        }

        public string RedactText(List<Entity> entityList, string document)
        {
            foreach(var entity in entityList)
            {
                document = document.Replace(entity.EntityText, new string('*', entity.EntityText.Length));
            }
            return document;
        }
    }
}
