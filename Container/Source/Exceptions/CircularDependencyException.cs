using System;

namespace IoC.Source.Exceptions
{
    /// <summary>
    /// Indicates that a type is trying to be resolved that has a circular dependency in the dependency chain.
    /// </summary>
    internal class CircularDependencyException : Exception
    {
        public CircularDependencyException(Type contract) 
            : base($"Circular dependency detected when trying to resolve: {contract.FullName}")
        {
        }
    }
}
