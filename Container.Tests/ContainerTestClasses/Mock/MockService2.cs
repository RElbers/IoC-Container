namespace IoC.Tests.ContainerTestClasses.Mock
{
    class MockService2 : IMockService
    {
        public string Name { get; set; } = "Service2";
        public string Host { get; set; } = "127.0.0.1";
    }
}
