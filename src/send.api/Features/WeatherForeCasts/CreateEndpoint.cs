using FastEndpoints;
using send.api.Shared.Exceptions;

namespace send.api.Features.WeatherForeCasts
{
    public class CreateEndpoint : Endpoint<CreateRequest, CreateResponse>
    {
        public override void Configure()
        {
            Post("weatherforecast");
            Summary(s =>
            {
                s.Summary = "Returns the current weather forecast";
                s.Description = "Gets the current weather forecast";
                s.Response<GetResponse>(201, "Success");
            });
            AllowAnonymous(); // Remove this line to require authentication
        }

        public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
        {
            //throw new DomainException(ConflictCode.already_exist, "Name already exists");
            var response = new CreateResponse
            {
                Id = 2,
                Message = "Weather forecast created successfully"
            };

            await SendAsync(response);
        }
    }
}
