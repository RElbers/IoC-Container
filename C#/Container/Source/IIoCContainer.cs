using System;

namespace IoC.Source
{
    public interface IIoCContainer
    {
        void RegisterType<TContract, TImplementation>() where TImplementation : TContract;
        void RegisterInstance<TContract>(TContract instance);
        void RegisterFunc<TContract>(Func<TContract> func);
        T Resolve<T>();
    }
}
