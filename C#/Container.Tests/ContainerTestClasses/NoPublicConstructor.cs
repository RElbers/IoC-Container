using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class NoPublicConstructor
    {
        public MockService1 Service1 { get; set; }
    
        private NoPublicConstructor( MockService1 service1)
        {
            Service1 = service1;
        }
    }
}
