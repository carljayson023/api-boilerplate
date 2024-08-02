namespace send.api.Shared.Authentication.Config
{
    public static class AspNetCoreChassisConfigureServices
    {
        public static IServiceCollection AddChassis(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<ConfigurationBuilder> configurationBuilderHandler = null
            )
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(services);
            configurationBuilderHandler?.Invoke(configurationBuilder);
            return services;
        }
    }
}
