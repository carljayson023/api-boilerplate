using send.api.Domain;
using System.Collections;

namespace send.api.ServiceManager
{
    public class WeatherForeCastsService : IWeatherForeCastsService
    {
        public async Task<WeatherForeCast> GetWeatherForeCasts()
        {
            var response = new WeatherForeCast
            {
                Date = DateTime.UtcNow,
                TemperatureC = new Random().Next(-20, 55),
                Summary = "Sunny"
            };

            return response;
        }
    }
}
