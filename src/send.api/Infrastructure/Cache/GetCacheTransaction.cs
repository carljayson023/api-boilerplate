using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace send.api.Infrastructure.Cache
{
    public interface IGetCacheTransaction
    {
        Task<T> GetCacheTransactions<T>(string tranxId, CancellationToken cancellationToken = default);
    }

    public class GetCacheTransaction : IGetCacheTransaction
    {
        private readonly IDistributedCache _distributedCache;

        public GetCacheTransaction(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> GetCacheTransactions<T>(string tranxId, CancellationToken cancellationToken = default)
        {
            string? cacheMember = await _distributedCache.GetStringAsync(tranxId, cancellationToken);

            if (string.IsNullOrEmpty(cacheMember))
                return default;

            return JsonConvert.DeserializeObject<T>(cacheMember);
        }
    }
}
