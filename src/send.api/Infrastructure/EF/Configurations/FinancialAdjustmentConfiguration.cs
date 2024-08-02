using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using send.api.Domain;

namespace send.api.Infrastructure.EF.Configurations
{
    /// <summary>
    /// Db table configuration: setup the name and attribute/properties of table
    /// </summary>
    public class FinancialAdjustmentConfiguration : IEntityTypeConfiguration<WeatherForeCast>
    {
        public void Configure(EntityTypeBuilder<WeatherForeCast> builder)
        {
            builder.ToTable("<name of table>").HasKey(type => type.Id);

            builder.Property(type => type.Date).HasColumnName("id");
            builder.Property(type => type.TemperatureC).HasColumnName("TemperatureC");
            builder.Property(type => type.TemperatureF).HasColumnName("TemperatureF");
            builder.Property(type => type.Summary).HasColumnName("Summary");
            builder.Property(type => type.CreatedBy).HasColumnName("created_by");
            builder.Property(type => type.CreatedAt).HasColumnName("created_at");
            builder.Property(type => type.UpdatedBy).HasColumnName("updated_by");
            builder.Property(type => type.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
