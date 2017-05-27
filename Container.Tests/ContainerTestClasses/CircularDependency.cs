namespace IoC.Tests.ContainerTestClasses
{
    class CircularDependency
    {
        public CircularDependency Dependency { get; set; }

        public CircularDependency(CircularDependency dependency)
        {
            Dependency = dependency;
        }
    }
}
