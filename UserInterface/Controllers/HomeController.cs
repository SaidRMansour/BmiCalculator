using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
        if (height == null || weight == null || height == null && weight == null)
        {
            ViewBag.Alert = "En af dine indtasninger var tomme. Prøv igen";
            return View("Index");
        }

        // Creating an HTTP client instance
        var client = _clientFactory.CreateClient("MyClient");

        // Forming a GET request with query parameters
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://collector-service/Collector?height={height}&weight={weight}");
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();

        // Handling non-success status codes
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Alert = $"Noget er galt! Grunden: {response.ReasonPhrase}";
            return View("Index");
        }

        // Deserializing response content into BMI object and rounding off the BMI value
        var bmiObj = JsonConvert.DeserializeObject<BMI>(result);
        ViewBag.BMI = Math.Round(bmiObj.Bmi, 2);
        ViewBag.Status = bmiObj.Status;

        return View("Index");
    }

    // Action for handling errors with cache settings specified
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

