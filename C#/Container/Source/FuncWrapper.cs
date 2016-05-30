using System;

namespace IoC.Source
{
    /// <summary>
    /// Wraps a Func<T> so invoke can be called without passing a generic.
    /// </summary>
    /// <typeparam name="T">Type of Func<></typeparam>
    internal sealed class FuncWrapper<T> : IFuncWrapper
    {
        private readonly Func<T> _func;

        public FuncWrapper(Func<T> func)
        {
            _func = func;
        }

        public object Invoke()
        {
            return _func.Invoke();
        }
    }

    internal interface IFuncWrapper
    {
        object Invoke();
    }
}
