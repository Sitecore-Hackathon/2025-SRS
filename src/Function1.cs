
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json.Serialization;

namespace FunctionApp2
{
    public class TeamsNotificationFunction
    {
        private static readonly string webhookUrl = "<MS Teams webhook Url>";

        private readonly ILogger<TeamsNotificationFunction> _logger;

        public TeamsNotificationFunction(ILogger<TeamsNotificationFunction> logger)
        {
            _logger = logger;
        }


        [Function("SendTeamsNotification")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processing Teams notification request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            _logger.LogInformation("✅ requestBody = " + requestBody);

            var data = JsonSerializer.Deserialize<PublishStatus>(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            if (data == null)
            {
                return new BadRequestObjectResult("Invalid request payload.");
            }

            _logger.LogInformation("✅ Item Name = " + data.Item?.Name);

            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");

            var payload = new
            {
                type = "message",
                attachments = new[]
                {
                new
                {
                    contentType = "application/vnd.microsoft.card.adaptive",
                    content = new
                    {
                        schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                        type = "AdaptiveCard",
                        version = "1.4",
                        body = new object[]
                        {
                            new { type = "TextBlock", text = "📢 **Publish Status Update**", weight = "Bolder", size = "Medium" },
                            new { type = "TextBlock", text = $"📄 **Item:** {data.Item?.Name}", wrap = true },
                            new { type = "TextBlock", text = $"✅ **EventName:** {data.EventName}", wrap = true, color = "Good" },
                            new { type = "TextBlock", text = $"👤 **Published By:** {data.WebhookItemName}", wrap = true },
                            new { type = "TextBlock", text = $"🕒 **Timestamp:** {timestamp}", wrap = true, spacing = "Small" }
                        }
                    }
                }
            }
            };

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(webhookUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send message. Status Code: {response.StatusCode}, Response: {responseBody}");
                    return new StatusCodeResult(500);
                }
            }

            _logger.LogInformation("✅ Message sent successfully to Teams!");
            return new OkObjectResult("Message sent successfully to Teams!");
        }
    }

    public class PublishStatus
    {
        [JsonPropertyName("EventName")]
        public string? EventName { get; set; }

        [JsonPropertyName("Item")]
        public Item? Item { get; set; }

        [JsonPropertyName("WebhookItemId")]
        public Guid? WebhookItemId { get; set; }

        [JsonPropertyName("WebhookItemName")]
        public string? WebhookItemName { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("Language")]
        public string? Language { get; set; }

        [JsonPropertyName("Version")]
        public int? Version { get; set; }

        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("ParentId")]
        public Guid ParentId { get; set; }

        [JsonPropertyName("TemplateId")]
        public Guid TemplateId { get; set; }

        [JsonPropertyName("MasterId")]
        public Guid MasterId { get; set; }

        [JsonPropertyName("SharedFields")]
        public List<Field>? SharedFields { get; set; }

        [JsonPropertyName("UnversionedFields")]
        public List<Field>? UnversionedFields { get; set; }

        [JsonPropertyName("VersionedFields")]
        public List<VersionedField>? VersionedFields { get; set; }
    }

    public class Field
    {
        [JsonPropertyName("Id")]
        public Guid Id { get; set; }

        [JsonPropertyName("Value")]
        public string? Value { get; set; }
    }

    public class VersionedField : Field
    {
        [JsonPropertyName("Version")]
        public int? Version { get; set; }

        [JsonPropertyName("Language")]
        public string? Language { get; set; }
    }

}
