using FastEndpoints;

namespace send.api.Features.WeatherForeCasts
{
    public class UpdateEndpoint : Endpoint<UpdateRequest, UpdateResponse>
    {
        public override void Configure()
        {
            Put("weatherforecast/{id}");
            Summary(s =>
            {
                s.Summary = "Returns the current weather forecast";
                s.Description = "Gets the current weather forecast";
                s.Response<GetResponse>(200, "Success");
            });
            AllowAnonymous(); // Remove this line to require authentication
        }

        public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
        {
            var id = 1;// Route<string>("id"); // Access the route parameter

            var response = new UpdateResponse
            {
                Message = $"Weather forecast with ID {id} updated successfully"
            };

            await SendAsync(response);
        }
    }
}
