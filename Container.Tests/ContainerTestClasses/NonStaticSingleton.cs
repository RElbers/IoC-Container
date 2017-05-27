namespace IoC.Tests.ContainerTestClasses
{
    class NonStaticSingleton
    {
        public static int NConstructed { get; set; }

        public NonStaticSingleton()
        {
            NConstructed++;
        }
    }
}
