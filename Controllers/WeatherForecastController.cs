using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace ElasticSearchLogs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger _logger;

        public WeatherForecastController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            _logger.Information("this is a request!");
            try
            {
                var rng = new Random();
                if (rng.Next(0, 5) < 2)
                {
                    throw new Exception("Oops what happened?");
                }

                return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
                            {
                                Date = DateTime.Now.AddDays(index),
                                TemperatureC = Random.Shared.Next(-20, 55),
                                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                            }).ToArray()
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "something bad happened!");
                return new StatusCodeResult(500);
            }
        }
    }
}