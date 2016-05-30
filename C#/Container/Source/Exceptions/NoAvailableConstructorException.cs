using System;

namespace IoC.Source.Exceptions
{
    internal class NoAvailableConstructorException : Exception
    {
        public NoAvailableConstructorException(Type contract) : base($"The type that is trying to be resolved does not have a constructor available for injection: {contract.FullName}")
        {
        }
    }
}
