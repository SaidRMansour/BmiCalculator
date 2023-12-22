using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Newtonsoft.Json;
using SharedModels;
using UserInterface.Models;

namespace UserInterface.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public HomeController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> CalculateBMI(string height, string weight)
    {
        // Validation for null input values
        if (height == null || weight == null || height == null && weight == null || !double.TryParse(height, out double parsedHeight))
        {
            ViewBag.Alert = "En af dine indtasninger var tomme. Prøv igen";
            MonitorService.Log.Here().Warning("Tom input: højde eller vægt er ikke angivet.");
            return View("Index");
        }
        try
        {
            // Parsing from Cm to M
            double heightMeter = parsedHeight / 100;

            // Creating an HTTP client instance
            var client = _clientFactory.CreateClient("MyClient");

            // Forming a GET request with query parameters
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://collector-service/Collector?height={heightMeter}&weight={weight}");
            MonitorService.Log.Here().Debug("Sender HTTP-anmodning til Collector-service: {RequestUri}", request.RequestUri);


            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            // Handling non-success status codes
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Alert = $"Noget er galt! Grunden: {response.ReasonPhrase}";
                MonitorService.Log.Here().Error("Fejl under HTTP-anmodning: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                return View("Index");
            }

            // Deserializing response content into BMI object and rounding off the BMI value
            var bmiObj = JsonConvert.DeserializeObject<BMI>(result);
            ViewBag.BMI = Math.Round(bmiObj.Bmi, 2);
            ViewBag.Status = bmiObj.Status;
            MonitorService.Log.Here().Debug("BMI beregnet: {BMI}, Status: {Status}", ViewBag.BMI, ViewBag.Status);

            return View("Index");
        }
        catch (Exception e)
        {
            MonitorService.Log.Here().Error("En fejl opstod under BMI beregningen: {Message}", e.Message);
            return View("Error");
        }
    }

    // Action for handling errors with cache settings specified
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        MonitorService.Log.Here().Error("Fejl opstået: RequestId {requestId}", requestId);
        return View(new ErrorViewModel { RequestId = requestId });
    }
}

