using FastEndpoints;
using send.api.Domain;
using send.api.ServiceManager;
using IMapper = AutoMapper.IMapper;

namespace send.api.Features.WeatherForeCasts
{
    public class GetEndpoint : EndpointWithoutRequest<GetResponse>
    {
        private readonly IWeatherForeCastsService _weatherForeCastsService;

        private readonly IMapper _mapper;

        public GetEndpoint(IWeatherForeCastsService weatherForeCastsService, IMapper mapper)
        {
            _weatherForeCastsService = weatherForeCastsService;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Get("weatherforecast");
            Version(1);
            Summary(s =>
            {
                s.Summary = "Returns the current weather forecast";
                s.Description = "Gets the current weather forecast";
                s.Response<GetResponse>(200, "Success");
            });
            Roles("taptap"); // uncomment this if enabled the Keyclok
            //AllowAnonymous(); // Remove this line to require authentication
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            WeatherForeCast foreCasts = await _weatherForeCastsService.GetWeatherForeCasts();

            var response = _mapper.Map<GetResponse>(foreCasts);

            await SendAsync(response);
        }

    }
}
