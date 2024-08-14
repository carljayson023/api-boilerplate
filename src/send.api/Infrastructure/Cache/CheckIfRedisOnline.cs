using Serilog;
using StackExchange.Redis;

namespace send.api.Infrastructure.Cache
{
    public static class CheckIfRedisOnline
    {
        #region Check if redis is online
        /// <summary>
        /// Checking if Redis Cache is Online
        /// </summary>
        /// <returns></returns>
        public static string IsRedisOnline(this string idempotencyKey, string agentCode)
        {
            try
            {
                string redisConnection = Environment.GetEnvironmentVariable("REDIS");
                string redisBaseKey = Environment.GetEnvironmentVariable("REDIS_BASE_KEY");
                int timeOut = int.Parse(Environment.GetEnvironmentVariable("REDIS_DELAY_TIME_OUT"));

                var task = Task.Run(() => CheckRedisConnection(redisConnection));

                // Wait for the task to complete or timeout after 2 seconds
                if (Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(timeOut))).Result.Equals(task) && task.Result.IsConnected)
                {
                    Log.Information($"Redis Connected: {task.Result.IsConnected}, Client Name: {task.Result.ClientName}, TimeoutMilliseconds: {task.Result.TimeoutMilliseconds}.");
                    return $"{redisBaseKey}:{agentCode}:{idempotencyKey}";
                }
                else
                {
                    Log.Warning($"Checking for a Redis connection is taking more than {timeOut} seconds. Redis timeout occurred.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Unable to connect to Redis. Error: {ex.Message}");
                return string.Empty;
            }
        }
        private static ConnectionMultiplexer CheckRedisConnection(string redisConnection)
        {
            return ConnectionMultiplexer.Connect(redisConnection);
        }

        #endregion
    }
}
