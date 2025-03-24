using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using pcms.Domain.Config;
using pcms.Domain.Interfaces;
using StackExchange.Redis;

namespace pcms.Application.Services
{
    public class RedisCacheService : ICacheService
    {
        private StackExchange.Redis.IDatabase _db;
        public static IConfiguration _configuration;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(ILogger<RedisCacheService> logger)
        {
            _logger = logger;
           // AppSettings appSettings = ConfigSettings.AppSettingsConfig;

            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(ConfigSettings.AppSettings.RedisUrl);
            });

            _db = lazyConnection.Value.GetDatabase();
        }

        public string GetData(string key)
        {
            try
            {
                if (_db.KeyExists(key))
                {
                    string value = _db.StringGet(key, CommandFlags.None);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}, {ex.StackTrace}");
                return string.Empty;
            }
            return default;
        }

        public async Task<string> GetDataAsync(string key)
        {
            try
            {
                if (_db.KeyExists(key))
                {
                    return await _db.StringGetAsync(key);
                }
                return string.Empty;
            }
            catch (RedisException ex)
            {
                _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
                return default;
                // throw ex.InnerException;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}, {ex.StackTrace}");
                return string.Empty;
            }
        }

        public bool SetData(string key, string value, int duration)
        {
            try
            {
                //  var json = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var isSet = _db.StringSet(key, value, TimeSpan.FromSeconds(duration));

                return isSet;
            }
            catch (RedisException ex)
            {
                _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
                return false;
                //  throw ex;
            }
        }

        //public async Task<bool> SetData<T>(string key, T value, int timespan)
        //{
        //    try
        //    {
        //        var json = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //        var isSet = await _db.StringSetAsync(key, json, TimeSpan.FromSeconds(timespan));

        //        return isSet;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
        //        return default;
        //        // throw ex;
        //    }
        //}

        public async Task<bool> SetDataAsync(string key, string value, int timespan)
        {
            try
            {
                //  var json = JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var isSet = await _db.StringSetAsync(key, value, TimeSpan.FromSeconds(timespan));

                return isSet;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
                return false;
            }
        }

        public void RemoveData(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    bool _isKeyExist = _db.KeyExists(key);

                    if (_isKeyExist == true)
                    {
                        _db.KeyDelete(key);
                    }
                    // return false;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
                    //  return false;
                }
            }
            // return false;
        }

        public async Task RemoveDataAsync(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    bool _isKeyExist = _db.KeyExists(key);

                    if (_isKeyExist == true)
                    {
                        _ = _db.KeyDeleteAsync(key);
                    }
                    //  return false;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error Message: {ex.Message}, {ex.StackTrace}");
                    //  return false;
                }
            }
            //  return false;
        }
    }
}
