using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class MultipleDependenciesAndInterface
    {
        public IMockService InterfaceService { get; set; }
        public IMockService Service1 { get; set; }
        public IMockService Service2 { get; set; }

        public MultipleDependenciesAndInterface(IMockService iService, MockService1 service1, MockService2 service2)
        {
            InterfaceService = iService;
            Service1 = service1;
            Service2 = service2;
        }
    }
}
