using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace Calculator.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{

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

        // Initialize BMI and status variables
        double bmi;
        string status;

        try
        {
            // Perform BMI calculation
            bmi = CalculateBmi(parsedWeight, parsedHeight);
            status = BmiStatus(bmi);
        }
        catch(Exception e)
        {
            // Handle any exceptions that occur during the calculation process
            return StatusCode(500, $"En intern serverfejl opstod: {e.Message}");
        }


        // Create and return a BMI object with the calculated values
        var bmiObject = new BMI
        {
            Bmi = bmi,
            Status = status
        };

        return Ok(bmiObject);
    }

    private double CalculateBmi(double weight, double height)
    {
        // BMI calculation formula: weight (kg) / (height (m))^2
        return weight / Math.Pow(height, 2);
    }

    private string BmiStatus(double bmi)
    {
        // Switch statement to determine the BMI category based on BMI value
        switch (bmi)
        {
            case double value when value < 18.5:
                return "Undervægtig";
            case double value when value >= 18.5 && value <= 24.9:
                return "Normalvægt";
            case double value when value >= 25 && value <= 29.9:
                return "Overvægtig";
            case double value when value >= 30 && value <= 34.9:
                return "Fedme klasse 1";
            case double value when value >= 35 && value <= 39.9:
                return "Fedme klasse 2";
            case double value when value >= 40:
                return "Ekstrem fedme (Fedme klasse 3)";
            default:
                return "Ugyldig BMI";
        }
    }

}

