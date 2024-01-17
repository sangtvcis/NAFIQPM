﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Repository.Implement
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IMemoryCache _cache;
        public CacheRepository(IMemoryCache cache)
        {
            _cache = cache;
        }
        public T Get<T>(string key)
        {
            T data = default;
            if (string.IsNullOrEmpty(key)) return data;
            data = (T)_cache.Get(key);
            return data;
        }

        public bool Set<T>(T data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key)) return false;
            _cache.Set(key, data);
            return true;
        }

        public bool Update<T>(T data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key)) return false;

            Remove(key);

            return Set(data, key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(key.Trim()))
            {
                _cache.Remove(key);
            }
        }
    }
}
