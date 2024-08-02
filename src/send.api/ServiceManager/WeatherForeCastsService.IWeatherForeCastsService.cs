using send.api.Domain;

namespace send.api.ServiceManager
{
    public interface IWeatherForeCastsService
    {
        Task<WeatherForeCast> GetWeatherForeCasts();
    }
}
