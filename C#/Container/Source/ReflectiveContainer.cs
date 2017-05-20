using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC.Source
{
    public class ReflectiveContainer : AbstractContainer
    {
        protected override object CreateObject(Type type, ConstructorInfo ctor, IEnumerable<ParameterInfo> ctorParams, HashSet<Type> history)
        {
            // If there are no parameters we can resolve this immediately. Otherwise call resolve recursively.
            var pars = ctorParams as ParameterInfo[] ?? ctorParams.ToArray();
            var obj = !pars.Any()
                ? Activator.CreateInstance(type)
                : ctor.Invoke(pars.Select(
                        parameterInfo => Resolve(parameterInfo.ParameterType, history)).ToArray());

            return obj;
        }
    }
}