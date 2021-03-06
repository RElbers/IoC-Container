using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IoC.Source;
using IoC.Source.Exceptions;
using IoC.Tests.ContainerTestClasses;
using IoC.Tests.ContainerTestClasses.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Tests
{
    [TestClass]
    public class ReflectiveContainerTests : BaseContainerTests
    {
        [TestInitialize]
        public void Init()
        {
            Container = new ReflectiveContainer();
        }
    }

    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public void CompareContainerPerformance()
        {
            var c = new CompiledContainer();
            var r = new ReflectiveContainer();
            c.RegisterType<IMockService, MockService1>();
            r.RegisterType<IMockService, MockService1>();
            GetValue<MultipleDependenciesAndInterface>(c);
            GetValue<MultipleDependenciesAndInterface>(r);

            Trace.WriteLine("Compiled");
            Trace.WriteLine(GetValue<MultipleDependenciesAndInterface>(c));
            Trace.WriteLine("Reflection");
            Trace.WriteLine(GetValue<MultipleDependenciesAndInterface>(r));
        }

        private static double GetValue<T>(IIoCContainer container)
        {
            var times = new List<TimeSpan>();
            for (int i = 0; i < 10000; i++)
            {
                var t = Time(() => container.Resolve<T>());
                times.Add(t);
            }

            return times.Average(t => t.TotalMilliseconds);
        }

        public static TimeSpan Time(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }

    [TestClass]
    public class CompiledContainerTests : BaseContainerTests
    {
        [TestInitialize]
        public void Init()
        {
            Container = new CompiledContainer();
        }

       
    }

    public class BaseContainerTests
    {
        protected IIoCContainer Container;


        [TestCleanup]
        public void CleanUp()
        {
            NonStaticSingleton.NConstructed = 0;
        }

        [TestMethod]
        public void Resolve_NoDependencies_NotRegistered()
        {
            var res = Container.Resolve<NoDependencies>();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Resolve_NoDependencies_Registered()
        {
            Container.RegisterType<NoDependencies, NoDependencies>();
            var res = Container.Resolve<NoDependencies>();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Resolve_Interface_Registered()
        {
            Container.RegisterType<IMockService, MockService1>();
            var res = Container.Resolve<InterfaceDependency>();

            Assert.IsNotNull(res.Service as MockService1);
        }

        [TestMethod]
        public void Resolve_MultipleDepencencies_NotRegistered()
        {
            var res = Container.Resolve<MultipleDependencies>();

            Assert.IsNotNull(res.Service2);
            Assert.IsNotNull(res.Service1);
        }

        [TestMethod]
        public void Resolve_MultipleDepencencies_AndInterface()
        {
            Container.RegisterType<IMockService, MockService1>();
            var res = Container.Resolve<MultipleDependenciesAndInterface>();

            Assert.IsNotNull(res.InterfaceService);
            Assert.IsNotNull(res.InterfaceService is MockService1);
            Assert.IsNotNull(res.Service2);
            Assert.IsNotNull(res.Service1);
        }

        [TestMethod]
        public void Resolve_InjectionAttributeConstructor()
        {
            var res = Container.Resolve<InjectConstructor>();

            Assert.IsNull(res.InterfaceService);
            Assert.IsNotNull(res.Service1);
            Assert.IsNotNull(res.Service2);
        }

        [TestMethod]
        public void Resolve_InjectionAttributeProperty()
        {
            var res = Container.Resolve<InjectProperty>();

            Assert.IsNull(res.InterfaceService);
            Assert.IsNotNull(res.Service1);
            Assert.IsNotNull(res.Service2);
        }

        [TestMethod]
        public void Resolve_Instance()
        {
            Container.RegisterInstance(Container.Resolve<NonStaticSingleton>());
            Container.Resolve<NonStaticSingleton>();
            Assert.AreEqual( 1, NonStaticSingleton.NConstructed);
            Container.Resolve<NonStaticSingleton>();
            Assert.AreEqual(1, NonStaticSingleton.NConstructed);
            Container.Resolve<NonStaticSingleton>();
            Assert.AreEqual(1, NonStaticSingleton.NConstructed);
        }

        [TestMethod]
        public void Resolve_Func()
        {
            int n = 0;
            Container.RegisterFunc(() =>
            {
                n++;
                return new NoDependencies();
            });

            var res1 = Container.Resolve<NoDependencies>();
            Assert.IsNotNull(res1);
            Assert.AreEqual(n, 1);
            var res2 = Container.Resolve<NoDependencies>();
            Assert.IsNotNull(res2);
            Assert.AreEqual(n, 2);
            var res3 = Container.Resolve<NoDependencies>();
            Assert.IsNotNull(res3);
            Assert.AreEqual(n, 3);
        }

        [TestMethod]
        public void Resolve_DoubleSameDependency()
        {
            Container.RegisterType<IMockService, MockService1>();

            var res1 = Container.Resolve<DoubleSameDependency>();
            Assert.IsNotNull(res1);
        }


        #region Exceptions
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolve_NullInstance()
        {
            Container.RegisterInstance<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolve_NullFunc()
        {
            Container.RegisterFunc<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void Resolve_CircularDependency_ThrowsCircularDependencyException()
        {
            Container.Resolve<CircularDependency>();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException))]
        public void Resolve_CircularSetter_ThrowsCircularDependencyException()
        {
            Container.RegisterType<IMockService, MockService1>();
            Container.Resolve<CircularSetter>();
        }

        [TestMethod]
        [ExpectedException(typeof(NotRegisteredException))]
        public void Resolve_Interface_NotRegistered_ThrowsNotRegisteredException()
        {
            Container.Resolve<InterfaceDependency>();
        }

        [TestMethod]
        [ExpectedException(typeof(NoAvailableConstructorException))]
        public void Resolve_NoConstructor_ThrowsNoPublicConstructorException()
        {
            Container.Resolve<NoPublicConstructor>();
        }

    }
    #endregion Exceptions
}



