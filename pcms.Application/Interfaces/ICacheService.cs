namespace pcms.Domain.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// Get Data using key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetData(string key);

        /// <summary>
        /// Get Data using key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>The data from the cache</returns>
        Task<string> GetDataAsync(string key);


        /// <summary>
        /// Set data with value and expiration time of Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationDate">Date and time of expiration</param>
        /// <returns>Boolean indicating failure or success</returns>
        bool SetData(string key, string value, int timeSpan = 30);

        /// <summary>
        /// Set data with value and expiration time of Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <returns>Boolean indicating failure or success</returns>
        Task<bool> SetDataAsync(string key, string value, int timespan = 30);


        /// <summary>
        /// Remove Data
        /// </summary>
        /// <param name="key"></param>
        void RemoveData(string key);

        /// <summary>
        /// Remove Data
        /// </summary>
        /// <param name="key"></param>
        Task RemoveDataAsync(string key);
    }
}
