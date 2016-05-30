import Annotations.InjectConstructor;
import Annotations.InjectSetter;
import Exceptions.CircularDependencyException;
import Exceptions.NoAvailableConstructorException;
import Exceptions.NotRegisteredException;

import java.lang.annotation.Annotation;
import java.lang.reflect.*;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.function.Supplier;

/**
 * Created by user on 21/9/2015.
 */
public class IoCContainer {
    private final HashMap<Type, Type> _typeToType = new HashMap<>();
    private final HashMap<Type, Object> _typeToInstance = new HashMap<>();
    private final HashMap<Type, Supplier> _typeToFunc = new HashMap<>();

    /**
     * Registers a type to be available for resolving.
     * Only interfaces and abstract classes need to be registered.
     *
     * @param from Registers this type so it is available for resolving.
     * @param to   Type that implements from.
     */
    public void registerType(Type from, Type to) {
        _typeToType.put(from, to);
    }

    public void registerInstance(Object instance) {
        if (instance == null)
            throw new IllegalArgumentException("registerInstance(Object instance): instance  can not be null.");
        _typeToInstance.put(instance.getClass(), instance);
    }

    public void registerFunc(Type from, Supplier to) {
        if (to == null)
            throw new IllegalArgumentException("registerFunc(Type from, Supplier to): func  can not be null.");
        _typeToFunc.put(from, to);
    }

    /**
     * Creates an instance of Type. Resolves all dependencies automatically.
     *
     * @param type Type of instance to create;
     * @return an instance of Type.
     * @throws CircularDependencyException     if a circular dependency is detected.
     * @throws NoAvailableConstructorException if a type can not be resolved because there is no constructor available to invoke. (e.g. abstract class or private constructor).
     * @throws NotRegisteredException          when trying to resolve an interface without registering an implementation.
     */
    public Object resolve(Type type) throws CircularDependencyException, NoAvailableConstructorException, NotRegisteredException {
        List<Type> resolvedTypes = new ArrayList<>();
        return resolve(type, resolvedTypes);
    }

    private Object resolve(Type contract, List<Type> resolvedTypes) throws CircularDependencyException, NoAvailableConstructorException, NotRegisteredException {
        // Check if we aren't resolving anything twice.
        if (resolvedTypes.contains(contract))
            throw new CircularDependencyException(contract);

        // If the type is registered as instance, return the instance.
        if (_typeToInstance.containsKey(contract))
            return _typeToInstance.get(contract);

        // If the type is registered as function, invoke the function and return the result.
        if (_typeToFunc.containsKey(contract))
            return _typeToFunc.get(contract).get();

        // Add contract to resolvedTypes so we can detect circular dependencies.
        resolvedTypes.add(contract);

        // Get the actual type to construct.
        Type implementation = getImplementation(contract);

        // Get class object
        Class<?> classObj = null;
        try {
            classObj = getClassFromType(implementation);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }

        Object result = constructorInjection(resolvedTypes, classObj);

        // Inject any properties that request injection.
        resolveProperties(result, classObj, resolvedTypes);

        // Remove contract from resolvedTypes. Only the current branch may not have duplicates.
        resolvedTypes.remove(contract);

        return result;
    }

    private Type getImplementation(Type contract) {
         // Add contract to _types if it is not known yet.
        if (!_typeToType.containsKey(contract)) {
            TryAddContract(contract);
        }
        // Get implementation of contract.
        return _typeToType.get(contract);
    }

    private void resolveProperties(Object object, Class classType, List<Type> resolvedTypes) {
        List<Method> injectionSetters = new ArrayList<>();

        // Get all methods with InjectSetter annotation.
        for (Method m : classType.getDeclaredMethods()) {
            // if method is annotated with @Test
            if (m.isAnnotationPresent(InjectSetter.class)) {
                injectionSetters.add(m);
            }
        }

        // Invoke every method and resolve it's dependencies.
        for (Method method : injectionSetters) {
            try {
                Parameter[] params = method.getParameters();
                Object[] paramsResolved = new Object[params.length];
                for (int i = 0; i < params.length; i++) {
                    paramsResolved[i] = resolve(params[i].getType(), resolvedTypes);
                }
                method.invoke(object, paramsResolved);
            } catch (IllegalAccessException | InvocationTargetException e) {
                e.printStackTrace();
            }
        }
    }

    private Object constructorInjection(List<Type> resolvedTypes, Class classObj) throws NoAvailableConstructorException, CircularDependencyException, NotRegisteredException {
        Object result = null;
        try {

            Constructor constructor = getConstructor(classObj);
            Parameter[] params = constructor.getParameters();
            // Constructor has no parameters. Return a new instance.
            if (params.length == 0) {
                result = constructor.newInstance();
            } else {
                Object[] paramsResolved = resolveParameters(params, resolvedTypes);
                result = constructor.newInstance(paramsResolved);
            }
        } catch (InstantiationException | IllegalAccessException | InvocationTargetException e) {
            e.printStackTrace();
        }
        return result;
    }

    private Constructor getConstructor(Class genericsType) {
        Constructor<?>[] constructors = genericsType.getConstructors();
        if (constructors.length == 0) {
            throw new NoAvailableConstructorException(genericsType);
        }

        for (Constructor constructor : constructors) {
            Annotation[] annotations = constructor.getDeclaredAnnotations();
            for (Annotation annotation : annotations) {
                if (annotation.annotationType().equals(InjectConstructor.class)) {
                    return constructor;
                }
            }
        }
        return constructors[0];
    }

    private Object[] resolveParameters(Parameter[] params, List<Type> resolvedTypes) throws NoAvailableConstructorException, CircularDependencyException, NotRegisteredException {
        Object[] paramDependencies = new Object[params.length];
        for (int i = 0; i < params.length; i++) {
            paramDependencies[i] = resolve(params[i].getParameterizedType(), resolvedTypes);
        }
        return paramDependencies;
    }

    private void TryAddContract(Type contract) throws NotRegisteredException {
        Class<?> genericsType = null;
        try {
            genericsType = getClassFromType(contract);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }
        int mods = genericsType.getModifiers();
        if (Modifier.isInterface(mods) || Modifier.isAbstract(mods)) {
            // If the type is an interface and it isn't in _types, throw an exception.
            throw new NotRegisteredException(contract);
        }
        // If the type is not in _types and it is not an interface. Add it to _types.
        _typeToType.put(contract, contract);
    }

    private Class getClassFromType(Type type) throws ClassNotFoundException {
        return Class.forName(getClassName(type));
    }

    private static String getClassName(Type type) {
        String fullName = type.toString();
        fullName.substring(1, fullName.indexOf(" "));
        return fullName.replaceAll(".* ", "");
    }

}
