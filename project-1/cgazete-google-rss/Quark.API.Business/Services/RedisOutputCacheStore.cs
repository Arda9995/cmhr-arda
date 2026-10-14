using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Quark.API.Business.Services
{
    public class RedisOutputCacheStore : IOutputCacheStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisOutputCacheStore> _logger;

        public RedisOutputCacheStore(IDistributedCache cache, ILogger<RedisOutputCacheStore> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        private string SanitizeKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;

            string sanitized = Regex.Replace(key, @"[^a-zA-Z0-9\-_]", "_");
            
            if (sanitized.Length > 250)
            {
                sanitized = sanitized.Substring(0, 250);
            }

            return sanitized;
        }
		private string ComputeSha256(string input)
		{
			using var sha256 = SHA256.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			byte[] hash = sha256.ComputeHash(bytes);
			var sb = new StringBuilder();
			foreach (var b in hash)
				sb.Append(b.ToString("x2"));
			return sb.ToString();
		}

		public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
		{
			try
			{
				var encodedKey = ComputeSha256(key);

				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogInformation("SetAsync was cancelled before execution started. Key: {Key}", key);
					return null;
				}

				return await _cache.GetAsync(encodedKey, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Redis cache read error for key: {Key}", key);
				return null;
			}
		}

		public async ValueTask SetAsync(string key, byte[] value, string[] tags, TimeSpan validFor, CancellationToken cancellationToken)
		{
			try
			{
				var encodedKey = ComputeSha256(key);
				var option = new DistributedCacheEntryOptions();

				if (validFor > TimeSpan.Zero)
				{
					option.SetAbsoluteExpiration(validFor);
				}
				else
				{
					option.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
				}

				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogInformation("SetAsync was cancelled before execution started. Key: {Key}", key);
					return;
				}
				await _cache.SetAsync(encodedKey, value, option, cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Redis cache write error for key: {Key}", key);
			}
		}


		public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                var sanitizedKey = SanitizeKey(key);
                await _cache.RemoveAsync(sanitizedKey, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis cache remove error for key: {Key}", key);
            }
        }

        public ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
        {
            _logger.LogWarning("EvictByTag is not implemented. Tag-based eviction requires custom logic.");
            return ValueTask.CompletedTask;
        }
    }
}
