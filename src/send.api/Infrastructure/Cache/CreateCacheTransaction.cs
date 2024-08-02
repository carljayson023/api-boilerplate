using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace send.api.Infrastructure.Cache
{
    public interface ICreateCacheTransaction
    {
        Task CreateCacheTransactions<T>(string TranxId, T value);
    }

    public class CreateCacheTransaction : ICreateCacheTransaction
    {
        private readonly IDistributedCache _distributedCache;

        public CreateCacheTransaction(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CreateCacheTransactions<T>(string TranxId, T value)
        {
            try
            {
                await _distributedCache.SetStringAsync(
                TranxId,
                JsonConvert.SerializeObject(value)
                );
            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }
    }
}
