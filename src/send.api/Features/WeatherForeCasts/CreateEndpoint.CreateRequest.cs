﻿

namespace send.api.Features.WeatherForeCasts
{
    public class CreateRequest
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
