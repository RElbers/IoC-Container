using System;
using System.Collections.Generic;

namespace IoC.Source
{
    internal class Cache<TKey, TVal>
    {
        private readonly Func<TKey, TVal> _factoryMethod;
        private readonly Dictionary<TKey, TVal> _cache = new Dictionary<TKey, TVal>();

        public Cache(Func<TKey, TVal> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public TVal this[TKey key]
        {
            get
            {
                if (_cache.ContainsKey(key))
                    return _cache[key];

                var val = _factoryMethod(key);
                _cache[key] = val;
                return val;
            }
            set => _cache[key] = value;
        }
    }
}
