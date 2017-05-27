namespace IoC.Tests.ContainerTestClasses.Mock
{
   public class MockService1 : IMockService
    {
        public string Name { get; set; } = "Service1";
       public string Host { get; set; } = "127.0.0.1";
    }
}
