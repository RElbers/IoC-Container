using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IoC.Source.Attributes;
using IoC.Source.Exceptions;

namespace IoC.Source
{
    public class IoCContainer : IIoCContainer
    {
        private readonly Dictionary<Type, Type> _typeToType = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _typeToInstance = new Dictionary<Type, object>();
        private readonly Dictionary<Type, IFuncWrapper> _typeToFunc = new Dictionary<Type, IFuncWrapper>();

        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     Only interfaces and abstract classes need to be registered.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <typeparam name="TImplementation">Type that implements TContract.</typeparam>
        public void RegisterType<TContract, TImplementation>() where TImplementation : TContract
        {
            _typeToType[typeof(TContract)] = typeof(TImplementation);
        }

        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     When resolving this type, the registered instance will be returned.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <param name="instance">This instance will be returned when resolving TContract</param>
        public void RegisterInstance<TContract>(TContract instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _typeToInstance[typeof(TContract)] = instance;
        }

        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     When resolving this type, func will be invoked and TResult will be returned.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <param name="func">This Func will be invoked and TResult will be returned when resolving TContract.</param>
        public void RegisterFunc<TContract>(Func<TContract> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            _typeToFunc[typeof(TContract)] = new FuncWrapper<TContract>(func);
        }

        /// <summary>
        ///     Creates an instance of T. Resolves all dependencies automatically.
        /// </summary>
        /// <typeparam name="T">Type to resolve.</typeparam>
        /// <returns>An instance of T</returns>
        public T Resolve<T>()
        {
            var resolvedTypes = new List<Type>();
            return (T)Resolve(typeof(T), resolvedTypes);
        }

        private object Resolve(Type contract, IList<Type> resolvedTypes)
        {
            // Check if we aren't resolving anything twice.
            if (resolvedTypes.Contains(contract))
                throw new CircularDependencyException(contract);

            // If the type is registered as instance, return the instance.
            if (_typeToInstance.ContainsKey(contract))
                return _typeToInstance[contract];

            // If the type is registered as Func, invoke the Func and return the result.
            if (_typeToFunc.ContainsKey(contract))
                return _typeToFunc[contract].Invoke();

            // Add contract to resolvedTypes so we can detect circular dependencies.
            resolvedTypes.Add(contract);

            // Get the actual type to construct.
            var implementation = GetImplementation(contract);

            var ctor = GetConstructor(implementation);
            ParameterInfo[] ctorParams = ctor.GetParameters();

            // If there are no parameters we can resolve this immediately. Otherwise call resolve recursively.
            var obj = ctorParams.Length == 0
                ? Activator.CreateInstance(implementation)
                : ctor.Invoke(ResolveParameters(ctorParams, resolvedTypes).ToArray());

            // Inject any properties that request injection.
            ResolveProperties(obj, resolvedTypes);

            // Remove contract from resolvedTypes. Only the current branch may not have duplicates.
            resolvedTypes.Remove(contract);

            return obj;
        }

        private Type GetImplementation(Type contract)
        {
            // Add contract to _types if it is not known yet.
            if (!_typeToType.ContainsKey(contract))
                TryAddContract(contract);

            // Get the implementation that belong to this type.
            return _typeToType[contract];
        }

        private void ResolveProperties(object obj, IList<Type> resolvedTypes)
        {
            // Get all properties of object.
            PropertyInfo[] properties = obj.GetType().GetProperties();

            // Get a list of all properties with the inject attribute.
            List<PropertyInfo> injectionProperties =
                properties.Where(property => property.GetCustomAttributes().OfType<InjectPropertyAttribute>().Any()).ToList();

            // Inject every property.
            foreach (var property in injectionProperties)
            {
                property.SetValue(obj, Resolve(property.PropertyType, resolvedTypes), null);
            }
        }

        private IEnumerable<object> ResolveParameters(IEnumerable<ParameterInfo> ctorParams, IList<Type> resolvedTypes)
        {
            return ctorParams.Select(parameterInfo => Resolve(parameterInfo.ParameterType, resolvedTypes)).ToList();
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
            ConstructorInfo[] constructors = implementation.GetConstructors();
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