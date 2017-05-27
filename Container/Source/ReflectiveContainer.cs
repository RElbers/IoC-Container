using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC.Source
{
    public class ReflectiveContainer : AbstractContainer
    {
        protected override object CreateObject(Type type, ConstructorInfo ctor, HashSet<Type> history)
        {
            // If there are no parameters we can resolve this immediately. Otherwise call resolve recursively.
            var ctorParams = ctor.GetParameters();
            var obj = !ctorParams.Any()
                ? Activator.CreateInstance(type)
                : ctor.Invoke(ctorParams.Select(
                        parameterInfo => Resolve(parameterInfo.ParameterType, history)).ToArray());

            return obj;
        }
    }
}