package Exceptions;

import java.lang.reflect.Type;

/**
 * Created by user on 22/9/2015.
 */
public class NotRegisteredException extends RuntimeException {
    public NotRegisteredException(Type contract) {
        super("Cannot resolve interface or abstract class if it has not been registered:" + contract.getTypeName());
    }
}
