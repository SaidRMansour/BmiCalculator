using Microsoft.AspNetCore.Mvc;
using Monitoring;
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
            MonitorService.Log.Here().Error("Ugyldig højde input: {height}", height);
            return BadRequest("Ugyldig højde.");
        }
        // Validate weight input
        if (!double.TryParse(weight, out double parsedWeight) || parsedWeight <= 0)
        {
            MonitorService.Log.Here().Error("Ugyldig vægt input: {weight}", weight);
            return BadRequest("Ugyldig vægt.");
        }

        MonitorService.Log.Here().Debug("Input valideret: Højde og vægt");

        try
        {
            // Create HTTP client instance
            var client = _clientFactory.CreateClient("MyClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://calculator-service/Calculator?height={parsedHeight}&weight={parsedWeight}");
            MonitorService.Log.Here().Debug("Opretter HTTP-anmodning: {Uri}", request.RequestUri);


            var retryPolicy = Policy
               .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
               .WaitAndRetryAsync(new[]
               {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
               });

            HttpResponseMessage response = await retryPolicy.ExecuteAsync(() => client.SendAsync(request));

            // Check if the response is successful
            if (!response.IsSuccessStatusCode)
            {
                MonitorService.Log.Here().Error("Fejl ved hentning af data: {ReasonPhrase}, Statuskode: {StatusCode}", response.ReasonPhrase, response.StatusCode);
                return StatusCode((int)response.StatusCode, $"Fejl ved hentning af data: {response.ReasonPhrase}");
            }

            MonitorService.Log.Here().Debug("Succesfuld respons modtaget: Statuskode {StatusCode}", response.StatusCode);

            // Deserialize the successful response
            var result = await response.Content.ReadAsStringAsync();
            var bmiObject = JsonConvert.DeserializeObject<BMI>(result);
            MonitorService.Log.Here().Debug("BMI objekt deserialiseret: {bmiObject}", bmiObject);

            // Return the deserialized BMI object
            return Ok(bmiObject);
        }
        catch (Exception e)
        {
            MonitorService.Log.Here().Error("Fejl under behandling: {Message}", e.Message);
            return StatusCode(500, $"En intern serverfejl opstod: {e.Message}");
        }

    }

}

