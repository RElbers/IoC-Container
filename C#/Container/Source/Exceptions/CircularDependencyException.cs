using System;

namespace IoC.Source.Exceptions
{
    internal class CircularDependencyException : Exception
    {
        public CircularDependencyException(Type contract) : base($"Circular dependency detected when trying to resolve: {contract.FullName}")
        {
        }
    }
}
