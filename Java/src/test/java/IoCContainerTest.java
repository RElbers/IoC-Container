import Exceptions.CircularDependencyException;
import Exceptions.NoAvailableConstructorException;
import Exceptions.NotRegisteredException;
import Mock.*;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.ExpectedException;

import java.util.concurrent.atomic.AtomicInteger;

import static org.junit.Assert.*;

/**
 * Created by user on 21/9/2015.
 */
public class IoCContainerTest {
    @Test
    public void resolve_NoDependencies_NotRegistered() {
        IoCContainer container = new IoCContainer();
        NoDependencies res = (NoDependencies) container.resolve(NoDependencies.class);

        assertNotNull(res);
    }

    @Test
    public void resolve_NoDependencies_Registered() {
        IoCContainer container = new IoCContainer();
        container.registerType(NoDependencies.class, NoDependencies.class);
        NoDependencies res = (NoDependencies) container.resolve(NoDependencies.class);

        assertNotNull(res);
    }

    @Test
    public void resolve_MultipleDependencies_NotRegistered() {
        IoCContainer container = new IoCContainer();
        MultipleDependencies res = (MultipleDependencies) container.resolve(MultipleDependencies.class);

        assertNotNull(res);
        assertNotNull(res.service1);
        assertNotNull(res.service2);
    }

    @Test
    public void resolve_Interface_Registered() {
        IoCContainer container = new IoCContainer();
        container.registerType(IMockService.class, MockService1.class);
        InterfaceDependency res = (InterfaceDependency) container.resolve(InterfaceDependency.class);

        assertNotNull(res.service);
        assertTrue(res.service instanceof MockService1);
    }

    @Test
    public void resolve_MultipleDependencies_AndInterface() {
        IoCContainer container = new IoCContainer();
        container.registerType(IMockService.class, MockService1.class);
        MultipleDependenciesAndInterface res = (MultipleDependenciesAndInterface) container.resolve(MultipleDependenciesAndInterface.class);

        assertNotNull(res.interfaceService);
        assertTrue(res.interfaceService instanceof MockService1);
        assertNotNull(res.service2);
        assertNotNull(res.service1);
    }

    @Test
    public void resolve_Instance() {
        IoCContainer container = new IoCContainer();
        container.registerInstance(container.resolve(NonStaticSingleton.class));

        container.resolve(NonStaticSingleton.class);
        assertEquals(1, NonStaticSingleton.nConstructed);
        container.resolve(NonStaticSingleton.class);
        assertEquals(1, NonStaticSingleton.nConstructed);
        container.resolve(NonStaticSingleton.class);
        assertEquals(1, NonStaticSingleton.nConstructed);
    }

    @Test
    public void resolve_InjectionAttributeConstructor() {
        IoCContainer container = new IoCContainer();
        InjectConstructorMock res = (InjectConstructorMock) container.resolve(InjectConstructorMock.class);

        assertNull(res.interfaceService);
        assertNotNull(res.service1);
        assertNotNull(res.service2);
    }


    @Test
    public void resolve_InjectionAttributeProperty() {
        IoCContainer container = new IoCContainer();
        InjectPropertyMock res = (InjectPropertyMock) container.resolve(InjectPropertyMock.class);

        assertNull(res.getInterfaceService());
        assertNotNull(res.getService1());
        assertNotNull(res.getService2());
    }

    @Test
    public void resolve_Func() {
        IoCContainer container = new IoCContainer();
        AtomicInteger num = new AtomicInteger (0);

        container.registerFunc(NoDependencies.class, () ->
        {
            num.incrementAndGet();
            return new NoDependencies();
        });

        NoDependencies res1 = (NoDependencies) container.resolve(NoDependencies.class);
        assertNotNull(res1);
        assertEquals(1, num.get());
        NoDependencies res2 = (NoDependencies) container.resolve(NoDependencies.class);
        assertNotNull(res2);
        assertEquals(2, num.get());
        NoDependencies res3 = (NoDependencies) container.resolve(NoDependencies.class);
        assertNotNull(res3);
        assertEquals(3, num.get());
    }

    @Rule
    public final ExpectedException circularDependencyException = ExpectedException.none();
    @Rule
    public final ExpectedException notRegisteredException = ExpectedException.none();
    @Rule
    public final ExpectedException illegalArgumentException = ExpectedException.none();
    @Rule
    public final ExpectedException noAvailableConstructorException = ExpectedException.none();

    @Test
    public void Resolve_NullFunc()
    {
        IoCContainer container = new IoCContainer();
        illegalArgumentException.expect(IllegalArgumentException.class);
        container.registerFunc(Object.class, null);
    }

    @Test
    public void resolve_NullInstance() {
        IoCContainer container = new IoCContainer();
        illegalArgumentException.expect(IllegalArgumentException.class);
        container.registerInstance(null);
    }

    @Test
    public void resolve_CircularDependency_ThrowsCircularDependencyException() {
        IoCContainer container = new IoCContainer();
        circularDependencyException.expect(CircularDependencyException.class);
        container.resolve(CircularDependency.class);
    }

    @Test
    public void Resolve_CircularDependency_ThrowsCircularDependencyException()
    {
        IoCContainer container = new IoCContainer();
        circularDependencyException.expect(CircularDependencyException.class);
        container.resolve(CircularDependency.class);
    }

    @Test
    public void Resolve_CircularSetter_ThrowsCircularDependencyException()
    {
        IoCContainer  container = new IoCContainer();
        container.registerType(IMockService.class, MockService1.class);
        circularDependencyException.expect(CircularDependencyException.class);
        container.resolve(CircularSetter.class);
    }


    @Test
    public void resolve_Interface_NotRegistered_ThrowsNotRegisteredException() {
        IoCContainer container = new IoCContainer();
        notRegisteredException.expect(NotRegisteredException.class);
        container.resolve(InterfaceDependency.class);
    }

    @Test
    public void resolve_NoConstructor_ThrowsNoPublicConstructorException() {
        IoCContainer container = new IoCContainer();
        noAvailableConstructorException.expect(NoAvailableConstructorException.class);
        container.resolve(NoPublicConstructor.class);
    }

}
