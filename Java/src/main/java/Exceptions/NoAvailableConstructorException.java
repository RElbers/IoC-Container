package Exceptions;

import java.lang.reflect.Type;

/**
 * Created by user on 22/9/2015.
 */
public class NoAvailableConstructorException extends RuntimeException {
    public NoAvailableConstructorException(Type contract) {
        super("The type that is trying to be resolved does not have a constructor available for injection: " + contract.getTypeName());
    }
}
