using Microsoft.EntityFrameworkCore;
using send.api.Domain;

namespace send.api.Infrastructure.EF
{
    public interface ISendDbContext
    {
        DbSet<WeatherForeCast> WeatherForeCasts { get; set; }
    }
}