using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Polly;
using SharedModels;

namespace Collector.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectorController : ControllerBase
{
    // Initialize the HTTP client factory
    private readonly IHttpClientFactory _clientFactory;

    public CollectorController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string height, string weight)
    {
        // Validate height input
        if (!double.TryParse(height, out double parsedHeight) || parsedHeight <= 0)
        {
            return BadRequest("Ugyldig højde.");
        }
        // Validate weight input
        if (!double.TryParse(weight, out double parsedWeight) || parsedWeight <= 0)
        {
            return BadRequest("Ugyldig vægt.");
        }

        try
        {
            // Create HTTP client instance
            var client = _clientFactory.CreateClient("MyClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"url"); // TODO

            var retryPolicy = Policy
               .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
               .WaitAndRetryAsync(new[]
               {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
               });

            HttpResponseMessage response = await retryPolicy.ExecuteAsync(() => client.SendAsync(request));

            //var response = await client.SendAsync(request);

            // Check if the response is successful
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Fejl ved hentning af data: {response.ReasonPhrase}");
            }

            // Deserialize the successful response
            var result = await response.Content.ReadAsStringAsync();
            var bmiObject = JsonConvert.DeserializeObject<BMI>(result);

            // Return the deserialized BMI object
            return Ok(bmiObject);
        }
        catch (Exception e)
        {
            // TODO -> Log message
            return StatusCode(500, $"Internal server error: {e.Message}");
        }

    }

}

