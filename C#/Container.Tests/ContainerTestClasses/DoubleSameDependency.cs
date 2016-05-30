using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoC.Tests.ContainerTestClasses.Mock;

namespace IoC.Tests.ContainerTestClasses
{
    class DoubleSameDependency1
    {
        private IMockService Service { get; set; }
    }

    class DoubleSameDependency2
    {
        public IMockService Service { get; set; }
    }

    class DoubleSameDependency
    {
        
        public DoubleSameDependency2 S2 { get; set; }
        public DoubleSameDependency1 S1 { get; set; }

        public DoubleSameDependency(DoubleSameDependency1 s1, DoubleSameDependency2 s2)
        {
            S1 = s1;
            S2 = s2;
        }
    }
}
