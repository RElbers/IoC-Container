using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class InterfaceDependency
    {
        public IMockService Service { get; set; }

        public InterfaceDependency(IMockService service)
        {
            Service = service;
        }

    }
}
