using System.Data.Common;
using System.Data;

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
            string dbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string intPayoutDb = Environment.GetEnvironmentVariable("IR_API_DB_CONNECTION");

            //Dapper PEPP Main DB
            //services.AddTransient<DbConnection>(_ => new MySqlConnection(peppMainServerDb));
            //services.AddTransient<IPEPPMainContext, PEPPMainContext>();

            ////Dapper IR Payout DB
            //services.AddTransient<IDbConnection>(_ => new MySqlConnection(intPayoutDb));
            //services.AddTransient<IIntPayoutContext, IntPayoutContext>();

            //EF
            //services.AddTransient<IIntPayoutDbContext, IntPayoutDbContext>();
            //services.AddDbContext<IntPayoutDbContext>(options => options.UseMySql(intPayoutDb, ServerVersion.AutoDetect(intPayoutDb)));

        }
    }
}
