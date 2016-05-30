package Exceptions;

import java.lang.reflect.Type;

/**
 * Created by user on 22/9/2015.
 */
public class CircularDependencyException extends RuntimeException {
    public CircularDependencyException(Type contract) {
        super("Circular dependency detected when trying to resolve: " + contract.getTypeName());
    }
}
