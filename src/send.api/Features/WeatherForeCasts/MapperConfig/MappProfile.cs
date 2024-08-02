using AutoMapper;
using send.api.Domain;
using send.api.Features.WeatherForeCasts;

namespace send.api.Features.WeatherForeCasts.MapperConfig
{
    public class MappProfile : Profile
    {
        public MappProfile()
        {
            CreateMap<CreateRequest, WeatherForeCast>();
            CreateMap<WeatherForeCast, CreateResponse>();

            CreateMap<GetRequest, WeatherForeCast>();
            CreateMap<WeatherForeCast, GetResponse>();
        }
    }
}
