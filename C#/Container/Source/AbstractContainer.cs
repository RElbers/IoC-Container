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
        private readonly Dictionary<Type, Type> _typeToType = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _typeToInstance = new Dictionary<Type, object>();
        private readonly Dictionary<Type, IFuncWrapper> _typeToFunc = new Dictionary<Type, IFuncWrapper>();

        /// <inheritdoc />
        public void RegisterType<TContract, TImplementation>() where TImplementation : TContract
        {
            _typeToType[typeof(TContract)] = typeof(TImplementation);
        }

        /// <inheritdoc />
        public void RegisterInstance<TContract>(TContract instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _typeToInstance[typeof(TContract)] = instance;
        }

        /// <inheritdoc />
        public void RegisterFunc<TContract>(Func<TContract> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            _typeToFunc[typeof(TContract)] = new FuncWrapper<TContract>(func);
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            var history = new HashSet<Type>();
            try
            {
                var obj = (T) Resolve(typeof(T), history);
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
            // Check if we aren't resolving anything twice.
            if (history.Contains(type))
                throw new CircularDependencyException(type);

            // If the type is registered as instance, return the instance.
            if (_typeToInstance.ContainsKey(type))
                return _typeToInstance[type];

            // If the type is registered as Func, invoke the Func and return the result.
            if (_typeToFunc.ContainsKey(type))
                return _typeToFunc[type].Invoke();

            // Add contract to resolvedTypes so we can detect circular dependencies.
            history.Add(type);

            // Get the actual type to construct.
            var implementation = GetImplementation(type);
            var ctor = GetConstructor(implementation);
            var ctorParams = ctor.GetParameters();
            var obj = CreateObject(implementation, ctor, ctorParams, history);

            // Inject any properties that request injection.
            ResolveProperties(obj, history);

            // Remove contract from resolvedTypes. Only the current branch may not have duplicates.
            history.Remove(type);

            return obj;
        }

        protected abstract object CreateObject
            (Type type, ConstructorInfo ctor, IEnumerable<ParameterInfo> ctorParams, HashSet<Type> resolvedTypes);

        private Type GetImplementation(Type contract)
        {
            // Add contract to _types if it is not known yet.
            if (!_typeToType.ContainsKey(contract))
                TryAddContract(contract);

            // Get the implementation that belong to this type.
            return _typeToType[contract];
        }

        private void ResolveProperties(object obj, HashSet<Type> resolvedTypes)
        {
            // Get all properties of object.
            var properties = obj.GetType().GetProperties();

            // Get a list of all properties with the inject attribute.
            var injectionProperties =
                properties.Where(p => p.GetCustomAttributes().OfType<InjectPropertyAttribute>().Any()).ToList();

            // Inject every property.
            foreach (var property in injectionProperties)
            {
                property.SetValue(obj, Resolve(property.PropertyType, resolvedTypes), null);
            }
        }

        private void TryAddContract(Type contract)
        {
            // If the type is an interface and it isn't in _types, throw an exception.
            if (contract.IsInterface || contract.IsAbstract)
                throw new NotRegisteredException(contract);

            // If the type is not in _types and it is not an interface. Add it to _types.
            _typeToType.Add(contract, contract);
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
    }
}
