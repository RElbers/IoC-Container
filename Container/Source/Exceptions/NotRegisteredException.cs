using System;

namespace IoC.Source.Exceptions
{
    /// <summary>
    /// Indicates that an interface or an abstract class is trying to be resolved that is not registered with the container.
    /// </summary>
    internal class NotRegisteredException : Exception
    {
        public NotRegisteredException(Type contract)
            : base($"Cannot resolve interface or abstract class if it has not been registered: {contract.FullName}")
        {
        }
    }
}
