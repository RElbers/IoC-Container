using IoC.Source.Attributes;
using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class InjectProperty
    {
        public IMockService InterfaceService { get; set; }
        [InjectProperty]
        public MockService1 Service1 { get; set; }
        [InjectProperty]
        public MockService2 Service2 { get; set; }
    }
}
