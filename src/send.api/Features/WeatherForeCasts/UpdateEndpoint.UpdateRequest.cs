namespace send.api.Features.WeatherForeCasts
{
    public class UpdateRequest
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
