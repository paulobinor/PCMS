using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(ILogger<MemoryCacheService> logger)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _logger = logger;
        }

        public string GetData(string key)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out _))
                {
                    _logger.LogInformation("Successfully retrieved data from memoryCache");
                    return _memoryCache.Get(key).ToString();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured while trying to get data from memoryCache! Error message: {ex.Message}, stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> GetDataAsync(string key)
        {
            try
            {
                return await Task.FromResult(GetData(key));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured while trying to get data from memoryCache! Error message: {ex.Message}, stack trace: {ex.StackTrace}");
                return string.Empty;
            }
        }

        public bool SetData(string key, string value, int timeSpan)
        {
            try
            {
                _memoryCache.Set(key, value, TimeSpan.FromSeconds(timeSpan));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured while trying to get data from memoryCache! Error message: {ex.Message}, stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> SetDataAsync(string key, string value, int timespan)
        {
            try
            {
                return await Task.FromResult(SetData(key, value, timespan));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured while trying to get data from memoryCache! Error message: {ex.Message}, stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public void RemoveData(string key)
        {
            try
            {
                _memoryCache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Error occured while trying to get data from memoryCache! Error message: {ex.Message}, stack trace: {ex.StackTrace}");
            }
            //return true;
        }

        public async Task RemoveDataAsync(string key) => RemoveData(key);
    }
}
