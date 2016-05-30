using System;
using IoC.Source;
using IoC.Source.Exceptions;
using IoC.Tests.ContainerTestClasses;
using IoC.Tests.ContainerTestClasses.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Tests
{
    [TestClass]
    public class IoCContainerTest
    {
        [TestMethod]
        public void Resolve_NoDependencies_NotRegistered()
        {
            var container = new IoCContainer();
            var res = container.Resolve<NoDependencies>();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Resolve_NoDependencies_Registered()
        {
            var container = new IoCContainer();
            container.RegisterType<NoDependencies, NoDependencies>();
            var res = container.Resolve<NoDependencies>();

            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void Resolve_Interface_Registered()
        {
            var container = new IoCContainer();
            container.RegisterType<IMockService, MockService1>();
            var res = container.Resolve<InterfaceDependency>();

            Assert.IsNotNull(res.Service as MockService1);
        }

        [TestMethod]
        public void Resolve_MultipleDepencencies_NotRegistered()
        {
            var container = new IoCContainer();
            var res = container.Resolve<MultipleDependencies>();

            Assert.IsNotNull(res.Service2);
            Assert.IsNotNull(res.Service1);
        }

        [TestMethod]
        public void Resolve_MultipleDepencencies_AndInterface()
        {
            var container = new IoCContainer();
            container.RegisterType<IMockService, MockService1>();
            var res = container.Resolve<MultipleDependenciesAndInterface>();

            Assert.IsNotNull(res.InterfaceService);
            Assert.IsNotNull(res.InterfaceService is MockService1);
            Assert.IsNotNull(res.Service2);
            Assert.IsNotNull(res.Service1);
        }

        [TestMethod]
        public void Resolve_InjectionAttributeConstructor()
        {
            var container = new IoCContainer();
            var res = container.Resolve<InjectConstructor>();

            Assert.IsNull(res.InterfaceService);
            Assert.IsNotNull(res.Service1);
            Assert.IsNotNull(res.Service2);
        }

        [TestMethod]
        public void Resolve_InjectionAttributeProperty()
        {
            var container = new IoCContainer();
            var res = container.Resolve<InjectProperty>();

            Assert.IsNull(res.InterfaceService);
            Assert.IsNotNull(res.Service1);
            Assert.IsNotNull(res.Service2);
        }

        [TestMethod]
        public void Resolve_Instance()
        {
            var container = new IoCContainer();
            container.RegisterInstance(container.Resolve<NonStaticSingleton>());
            container.Resolve<NonStaticSingleton>();
            Assert.AreEqual(NonStaticSingleton.NConstructed, 1);
            container.Resolve<NonStaticSingleton>();
            Assert.AreEqual(NonStaticSingleton.NConstructed, 1);
            container.Resolve<NonStaticSingleton>();
            Assert.AreEqual(NonStaticSingleton.NConstructed, 1);
        }

        [TestMethod]
        public void Resolve_Func()
        {
            var container = new IoCContainer();
            int n = 0;
            container.RegisterFunc<NoDependencies>(() =>
            {
                n++;
                return new NoDependencies();
            });

            var res1 = container.Resolve<NoDependencies>();
            Assert.IsNotNull(res1);
            Assert.AreEqual(n, 1);
            var res2 = container.Resolve<NoDependencies>();
            Assert.IsNotNull(res2);
            Assert.AreEqual(n, 2);
            var res3 = container.Resolve<NoDependencies>();
            Assert.IsNotNull(res3);
            Assert.AreEqual(n, 3);
        }

        [TestMethod]
        public void Resolve_DoubleSameDependency()
        {
            var container = new IoCContainer();
            container.RegisterType<IMockService, MockService1>();

            var res1 = container.Resolve<DoubleSameDependency>();
            Assert.IsNotNull(res1);
        }


        #region Exceptions
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Instance may not be null when registering instance.")]
        public void Resolve_NullInstance()
        {
            var container = new IoCContainer();
            container.RegisterInstance<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Func may not be null when registering instance.")]
        public void Resolve_NullFunc()
        {
            var container = new IoCContainer();
            container.RegisterFunc<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException), "A class with a circular dependency was trying to be resolved.")]
        public void Resolve_CircularDependency_ThrowsCircularDependencyException()
        {
            var container = new IoCContainer();
            container.Resolve<CircularDependency>();
        }

        [TestMethod]
        [ExpectedException(typeof(CircularDependencyException), "A class with a circular dependency was trying to be resolved.")]
        public void Resolve_CircularSetter_ThrowsCircularDependencyException()
        {
            var container = new IoCContainer();
            container.RegisterType<IMockService, MockService1>();
            container.Resolve<CircularSetter>();
        }

        [TestMethod]
        [ExpectedException(typeof(NotRegisteredException), "A class that wasn't registered was trying to be resolved.")]
        public void Resolve_Interface_NotRegistered_ThrowsNotRegisteredException()
        {
            var container = new IoCContainer();
            container.Resolve<InterfaceDependency>();
        }

        [TestMethod]
        [ExpectedException(typeof(NoAvailableConstructorException), "A class without constructors was trying to be resolved.")]
        public void Resolve_NoConstructor_ThrowsNoPublicConstructorException()
        {
            var container = new IoCContainer();
            container.Resolve<NoPublicConstructor>();
        }

    }
    #endregion Exceptions
}



