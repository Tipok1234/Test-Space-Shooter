using System;
using System.Collections.Generic;

namespace Managers
{
    public class DIContainer 
    {
        private readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            _bindings[typeof(T)] = instance;
        }

        public T Resolve<T>()
        {
            if (_bindings.TryGetValue(typeof(T), out var instance))
            {
                return (T)instance;
            }

            throw new Exception($"DIContainer: type {typeof(T).Name} is not registered");
        }

        public bool HasBinding<T>()
        {
            return _bindings.ContainsKey(typeof(T));
        }
    }
}