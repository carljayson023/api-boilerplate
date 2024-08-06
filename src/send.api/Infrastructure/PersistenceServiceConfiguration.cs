using System.Data.Common;
using System.Data;
using send.api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace send.api.Infrastructure
{
    public static class PersistenceServiceConfiguration
    {
        /// <summary>
        /// Register Db Connection String
        /// </summary>
        /// <param name="services"> Service Collection </param>
        public static void AddPersistenceInfrastructure(this IServiceCollection services)
        {
            //string dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sendApiConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            //Dapper PEPP Main DB
            //services.AddTransient<DbConnection>(_ => new MySqlConnection(peppMainServerDb));
            //services.AddTransient<IPEPPMainContext, PEPPMainContext>();

            ////Dapper IR Payout DB
            //services.AddTransient<IDbConnection>(_ => new MySqlConnection(intPayoutDb));
            //services.AddTransient<IIntPayoutContext, IntPayoutContext>();

            //EF
            //services.AddTransient<ISendDbContext, SendDbContext>();
            //services.AddDbContext<SendDbContext>(options => options.UseMySql(sendApiConnectionString, ServerVersion.AutoDetect(sendApiConnectionString)));

        }
    }
}
