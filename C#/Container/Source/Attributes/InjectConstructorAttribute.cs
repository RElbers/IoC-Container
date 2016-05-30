using System;

namespace IoC.Source.Attributes
{
    /// <summary>
    /// Marks an constructor for injection.
    /// When resolving the class this constructor will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectConstructorAttribute : Attribute
    {
    }

    /// <summary>
    /// Marks a property for injection.
    /// When resolving the class this property will be setter-injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectPropertyAttribute : Attribute
    {
    }
}
