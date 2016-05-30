using System;

namespace IoC.Source.Exceptions
{
    internal class NotRegisteredException : Exception
    {
        public NotRegisteredException(Type contract)
            : base($"Cannot resolve interface or abstract class if it has not been registered: {contract.FullName}")
        {
        }
    }
}
