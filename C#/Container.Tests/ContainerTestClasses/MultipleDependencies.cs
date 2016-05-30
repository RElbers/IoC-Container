using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class MultipleDependencies
    {
        public MockService1 Service1 { get; set; }
        public MockService2 Service2 { get; set; }

        public MultipleDependencies(MockService1 service1, MockService2 service2)
        {
            Service1 = service1;
            Service2 = service2;
        }
    }
}
