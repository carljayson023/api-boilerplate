using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

namespace send.api.Shared.Authentication.Config
{
    public interface IAuthentication<TOption>
      where TOption : AuthenticationSchemeOptions
    {
        IServiceCollection AddAuth(IServiceCollection services, Action<TOption> options);
    }

    public class JWTAuthentication : IAuthentication<JwtBearerOptions>
    {
        public JWTAuthentication() { }
        public IServiceCollection AddAuth(IServiceCollection services, Action<JwtBearerOptions> options)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options);

            return services;
        }
    }
}
