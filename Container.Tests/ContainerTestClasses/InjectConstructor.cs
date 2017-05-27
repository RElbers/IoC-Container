using IoC.Source.Attributes;
using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class InjectConstructor
    {
        public IMockService InterfaceService { get; set; }
        public IMockService Service1 { get; set; }
        public IMockService Service2 { get; set; }
        public string Str { get; set; }

        public InjectConstructor(IMockService iService, MockService1 service1, MockService2 service2)
        {
            InterfaceService = iService;
            Service1 = service1;
            Service2 = service2;
        }

        [InjectConstructor]
        public InjectConstructor(MockService1 service1, MockService2 service2)
        {
            Service1 = service1;
            Service2 = service2;
        }

        public InjectConstructor(IMockService iService, MockService2 service2, string str)
        {
            InterfaceService = iService;
            Service2 = service2;
            Str = str;  
        }

    }
}
