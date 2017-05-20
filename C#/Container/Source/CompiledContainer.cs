using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace IoC.Source
{
    public class CompiledContainer : AbstractContainer
    {
        private readonly Dictionary<Type, Func<object>> _cache = new Dictionary<Type, Func<object>>();
        private static readonly MethodInfo ResolveMethod = typeof(AbstractContainer).GetMethod("Resolve", BindingFlags.NonPublic | BindingFlags.Instance);

        protected override object CreateObject
            (Type type, ConstructorInfo ctor, IEnumerable<ParameterInfo> ctorParams, HashSet<Type> history)
        {
            // Check the cache for a compiled expression.
            if (_cache.ContainsKey(type))
                return _cache[type]();

            var f = Inner(ctor, ctorParams, history);

            _cache[type] = f;

            var obj = f();
            return obj;
        }

        private Func<object> Inner(ConstructorInfo ctor, IEnumerable<ParameterInfo> ctorParams, HashSet<Type> history)
        {
            var ps = new List<Expression>();
            foreach (var parameterInfo in ctorParams)
            {
                var p = parameterInfo.ParameterType;
                var pTypeE = Expression.Constant(p);
                var resolveE = ResolveMethod;
                var thisE = Expression.Constant(this);
                var historyE = Expression.Constant(history);
                var callResolveE = Expression.Call(thisE, resolveE, pTypeE, historyE);
                var castResultE = Expression.Convert(callResolveE, p);
                ps.Add(castResultE);
            }

            var exnew = Expression.New(ctor, ps);
            var lambda = Expression.Lambda(exnew);
            var compiled = lambda.Compile();

            return () => compiled.DynamicInvoke();
        }
    }
}
