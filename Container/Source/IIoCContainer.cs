using System;

namespace IoC.Source
{
    public interface IIoCContainer
    {
        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     Only interfaces and abstract classes need to be registered.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <typeparam name="TImplementation">Type that implements TContract.</typeparam> 
        void RegisterType<TContract, TImplementation>() where TImplementation : TContract;

        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     When resolving this type, the registered instance will be returned.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <param name="instance">This instance will be returned when resolving TContract</param> 
        void RegisterInstance<TContract>(TContract instance);

        /// <summary>
        ///     Registers a type to be available for resolving.
        ///     When resolving this type, func will be invoked and TResult will be returned.
        /// </summary>
        /// <typeparam name="TContract">Registers this type so it is available for resolving.</typeparam>
        /// <param name="func">This Func will be invoked and TResult will be returned when resolving TContract.</param>
        void RegisterFunc<TContract>(Func<TContract> func);

        /// <summary>
        ///     Creates an instance of T. Resolves all dependencies automatically.
        /// </summary>
        /// <typeparam name="T">Type to resolve.</typeparam>
        /// <returns>An instance of T</returns>
        T Resolve<T>();
    }
}
