using FastEndpoints;

namespace send.api.Features.Items
{
    public class GetItemEndpoint : EndpointWithoutRequest<GetResponse>
    {
        public override void Configure()
        {
            Verbs(Http.GET);
            Routes("items");
            Version(1);
            Summary(s =>
            {
                s.Summary = "Returns the current Items item";
                s.Description = "Gets the current Items item";
            });
            //Tags(new[] { "Items" });
            //Roles("show-remittance");
            AllowAnonymous(); // Remove this line to require authentication
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var response = new GetResponse
            {
                Date = DateTime.UtcNow,
                TemperatureC = new Random().Next(-20, 55),
                Summary = "Sunny"
            };

             //throw new Exception("hre++++++");
            await SendAsync(response);
        }

    }
}
