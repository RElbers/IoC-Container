using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IoC.Source.Attributes;
using IoC.Source.Exceptions;

namespace IoC.Source
{
    public abstract class AbstractContainer : IIoCContainer
    {
        private readonly Dictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _singletonMap = new Dictionary<Type, object>();
        private readonly Dictionary<Type, IFuncWrapper> _factoryMethodMap = new Dictionary<Type, IFuncWrapper>();

        private readonly Cache<Type, IEnumerable<PropertyInfo>> _propertyCache = new Cache<Type, IEnumerable<PropertyInfo>>(GetInjectProperties);
        private readonly Cache<Type, ConstructorInfo> _ctorCache = new Cache<Type, ConstructorInfo>(GetConstructor);

        /// <inheritdoc />
        public void RegisterType<TContract, TImplementation>() where TImplementation : TContract
        {
            _typeMap[typeof(TContract)] = typeof(TImplementation);
        }

        /// <inheritdoc />
        public void RegisterInstance<TContract>(TContract instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _singletonMap[typeof(TContract)] = instance;
        }

        /// <inheritdoc />
        public void RegisterFunc<TContract>(Func<TContract> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            _factoryMethodMap[typeof(TContract)] = new FuncWrapper<TContract>(func);
        }

        /// <inheritdoc />
        public virtual T Resolve<T>()
        {
            var history = new HashSet<Type>();
            try
            {
                var obj = (T)Resolve(typeof(T), history);
                return obj;
            }
            catch (Exception e)
            {
                // Because the exceptions can be wrapped in TargetInvocationExceptions,
                // I rethrow the InnerException to preserve the expected interface.
                // And no I don't like doing this, but it works.
                if (e.InnerException is CircularDependencyException ||
                    e.InnerException is NoAvailableConstructorException ||
                    e.InnerException is NotRegisteredException)
                    throw e.InnerException;
                throw;
            }
        }

        protected object Resolve(Type type, HashSet<Type> history)
        {
            // If the type is registered as instance, return the instance.
            if (_singletonMap.ContainsKey(type))
                return _singletonMap[type];

            // If the type is registered as Func, invoke the Func and return the result.
            if (_factoryMethodMap.ContainsKey(type))
                return _factoryMethodMap[type].Invoke();

            // Check for cyclic dependencies.
            if (history.Contains(type))
                throw new CircularDependencyException(type);
            history.Add(type);

            // Get the correct type and construct the object.
            var implementation = GetImplementation(type);
            var ctor = _ctorCache[implementation];
            var obj = CreateObject(implementation, ctor, history);

            // Inject any properties with InjectPropertyAttribute.
            InjectProperties(obj, history);

            // Remove contract from history. Only the current branch may not have duplicates.
            history.Remove(type);

            return obj;
        }

        protected abstract object CreateObject
            (Type type, ConstructorInfo ctor, HashSet<Type> resolvedTypes);

        private Type GetImplementation(Type contract)
        {
            // Is this contract registered in the mapping?
            if (!_typeMap.ContainsKey(contract))
            {
                // If the type is an interface and it isn't in the collection of types, throw an exception.
                if (contract.IsInterface || contract.IsAbstract)
                    throw new NotRegisteredException(contract);

                // A concrete type maps to itself.
                _typeMap.Add(contract, contract);
            }

            // Get the implementation that belong to this type.
            return _typeMap[contract];
        }

        private void InjectProperties(object obj, HashSet<Type> resolvedTypes)
        {
            var injectionProperties = _propertyCache[obj.GetType()];

            foreach (var property in injectionProperties)
            {
                property.SetValue(obj, Resolve(property.PropertyType, resolvedTypes), null);
            }
        }

        private static ConstructorInfo GetConstructor(Type implementation)
        {
            var constructors = implementation.GetConstructors();
            if (constructors.Length == 0)
                throw new NoAvailableConstructorException(implementation);

            // Check if there is a ctor that has InjectConstructorAttribute.
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttributes().OfType<InjectConstructorAttribute>().Any())
                    return ctor;
            }

            return constructors[0];
        }

        private static IEnumerable<PropertyInfo> GetInjectProperties(Type type)
        {
            // Get all properties of object.
            var properties = type.GetProperties();

            // Get a list of all properties with the inject attribute.
            var injectionProperties = properties.Where(p => p.GetCustomAttributes().OfType<InjectPropertyAttribute>().Any()).ToList();

            return injectionProperties;
        }
    }
}
