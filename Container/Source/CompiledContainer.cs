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
            (Type type, ConstructorInfo ctor,  HashSet<Type> history)
        {
            // Check the cache for a compiled expression.
            if (_cache.ContainsKey(type))
                return _cache[type]();

            var f = GenerateFactoryMethod(ctor, ctor.GetParameters(), history);

            _cache[type] = f;
            var obj = f();
            return obj;
        }

        private Func<object> GenerateFactoryMethod(ConstructorInfo ctor, IEnumerable<ParameterInfo> ctorParams, HashSet<Type> history)
        {
            var ctorParamExprs = new List<Expression>();
            foreach (var parameterInfo in ctorParams)
            {
                var pType = parameterInfo.ParameterType;
                var pTypeEpr = Expression.Constant(pType);
                var resolveMethodEpr = ResolveMethod;
                var thisExpr = Expression.Constant(this);
                var historyExpr = Expression.Constant(history);
                var callResolveExpr = Expression.Call(thisExpr, resolveMethodEpr, pTypeEpr, historyExpr);
                var castResultExpr = Expression.Convert(callResolveExpr, pType);
                ctorParamExprs.Add(castResultExpr);
            }

            var exnew = Expression.New(ctor, ctorParamExprs);
            var lambda = Expression.Lambda<Func<object>>(exnew);
            var compiled = lambda.Compile();
            return compiled;
        }
    }
}
