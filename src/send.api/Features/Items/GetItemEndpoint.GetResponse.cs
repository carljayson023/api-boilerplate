namespace send.api.Features.Items
{
    public class GetResponse
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureFes => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}