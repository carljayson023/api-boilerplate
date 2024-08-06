using Microsoft.EntityFrameworkCore;
using send.api.Domain;
using send.api.Infrastructure.EF.Configurations;

namespace send.api.Infrastructure.EF
{
    public class SendDbContext : DbContext, ISendDbContext
    {
        public DbSet<WeatherForeCast> WeatherForeCasts { get; set; }

        public SendDbContext(DbContextOptions<SendDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new WeatherForecastConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
