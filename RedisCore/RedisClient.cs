using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCore
{
    public class RedisClient
    {
        private static readonly object _syncRedis = new object();
        private TimeSpan _DefaultCacheTimeSpan = TimeSpan.FromDays(1);

        private static bool _IsRedisAlive = false;

        private static ConnectionMultiplexer _RedisClient;

        private static IDatabase db;

        /// <summary>
        /// 初始化Redis
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="pass"></param>
        public RedisClient(string host = "", int port = 0, string pass = "")
        {
            if (_RedisClient != null)
                return;

            if (string.IsNullOrEmpty(host))
            {
                host = "10.57.180.144";
            }
            if (port == 0)
            {
                port = 6379;
            }
            if (string.IsNullOrEmpty(pass))
            {
                pass = "111111";
            }
            _RedisClient = ConnectionMultiplexer.Connect(new StringBuilder().Append(host).Append(port).Append(pass).ToString());
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="key"></param>
        /// <param name="dbIndex"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(T objData, string key, int dbIndex, TimeSpan? expiry = null)
        {
            if (!_RedisClient.IsConnected)
            {
                return false;
            }
            if (objData == null)
                return false;

            db = _RedisClient.GetDatabase(dbIndex);
            var json = JsonConvert.SerializeObject(objData);
            return await db.StringSetAsync(key, json, expiry);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, int dbIndex)
        {
            if (!_RedisClient.IsConnected)
            {
                return default(T);
            }

            db = _RedisClient.GetDatabase(dbIndex);
            var resultString = await db.StringGetAsync(key);

            var result = JsonConvert.DeserializeObject<T>(resultString);
            return result;
        }

        /// <summary>
        /// 移除指定key
        /// </summary>
        /// <param name="dbIndex"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool RemoveAsync(int dbIndex, List<string> keys)
        {
            if (!_RedisClient.IsConnected)
            {
                return false;
            }

            db = _RedisClient.GetDatabase(dbIndex);

            if (keys.Count() == 1)
            {
                return db.KeyDelete(keys.FirstOrDefault());
            }
            else
            {
                foreach (var item in keys)
                {
                    db.KeyDelete(item);
                }
                return true;
            }
        }
    }
}
