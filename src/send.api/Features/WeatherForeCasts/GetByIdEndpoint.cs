using FastEndpoints;
using Microsoft.AspNetCore.Components;

namespace send.api.Features.WeatherForeCasts
{
    public class GetByIdEndpoint : EndpointWithoutRequest<GetByIdResponse>
    {
        public override void Configure()
        {
            Get("weatherforecast/{id}");
            Summary(s =>
            {
                s.Summary = "Returns the current weather forecast";
                s.Description = "Gets the current weather forecast";
                s.Response<GetResponse>(200, "Success");
            });
            AllowAnonymous(); // Remove this line to require authentication
        }
        
        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<int>("id");

            var response = new GetByIdResponse
            {
                Id = 1,
                Date = DateTime.UtcNow,
                TemperatureC = new Random().Next(-20, 55),
                Summary = "Sunny"
            };

            await SendAsync(response);
        }

        //public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        //{
        //    var response = new GetByIdResponse
        //    {
        //        Id = 1,
        //        Date = DateTime.UtcNow,
        //        TemperatureC = new Random().Next(-20, 55),
        //        Summary = "Sunny"
        //    };

        //    await SendAsync(response);
        //}


    }

}
