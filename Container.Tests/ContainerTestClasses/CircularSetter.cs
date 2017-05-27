using IoC.Source.Attributes;

namespace IoC.Tests.ContainerTestClasses
{
  public class CircularSetter2
    {
        [InjectProperty]
        public CircularSetter1 CircularSetter1 { get; set; }
    }
    public class CircularSetter1
    {

        [InjectProperty]
        public CircularSetter2 CircularSetter2 { get; set; }
    }
    public class CircularSetter
    {
        [InjectProperty]
        public CircularSetter1 CircSet1 { get; set; }
        [InjectProperty]
        public CircularSetter2 CircSet2 { get; set; }
    }

}
