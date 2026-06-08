using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
    }

    public enum Lifetime
    {
        Transient,
        Singleton
    }

    public class Container
    {
        private class Registration
        {
            public Type ImplementationType;
            public Lifetime Lifetime;
            public Func<Container, object> Factory; 
        }

        private readonly Dictionary<Type, Registration> _registry = new Dictionary<Type, Registration>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        public void Register<TInterface, TImplementation>(Lifetime lifetime = Lifetime.Singleton)
            where TImplementation : TInterface
        {
            _registry[typeof(TInterface)] = new Registration
            {
                ImplementationType = typeof(TImplementation),
                Lifetime = lifetime
            };
        }

        public void Register<T>(Func<Container, T> factory, Lifetime lifetime = Lifetime.Singleton)
        {
            _registry[typeof(T)] = new Registration
            {
                Lifetime = lifetime,
                Factory = container => factory(container)
            };
        }

        public void RegisterSingletonInstance<T>(T instance)
        {
            _registry[typeof(T)] = new Registration { Lifetime = Lifetime.Singleton };
            _singletons[typeof(T)] = instance;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        private object Resolve(Type type)
        {
            if (!_registry.TryGetValue(type, out var reg))
                throw new Exception($"Type {type.Name} no registered in DI Container");

            if (_singletons.TryGetValue(type, out var instance))
                return instance;

            object newInstance = reg.Factory != null
                ? reg.Factory(this)
                : CreateInstance(reg.ImplementationType);

            if (reg.Lifetime == Lifetime.Singleton)
                _singletons[type] = newInstance;

            return newInstance;
        }

        private object CreateInstance(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length == 0) return Activator.CreateInstance(type);

            var ctor = constructors[0];
            var parameters = ctor.GetParameters();
            var resolvedParams = parameters.Select(p => Resolve(p.ParameterType)).ToArray();

            return ctor.Invoke(resolvedParams);
        }
        
        public void Inject(MonoBehaviour mb)
        {
            var type = mb.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() == null) continue;

                if (_registry.ContainsKey(field.FieldType))
                {
                    field.SetValue(mb, Resolve(field.FieldType));
                }
                else
                {
                    Debug.LogWarning($"[Inject] Dependency not found {field.FieldType.Name} at {type.Name}");
                }
            }
        }
    }
}
