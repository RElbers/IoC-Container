package Mock;

/**
 * Created by user on 22/9/2015.
 */
public class CircularDependency {
    public CircularDependency Dependency;

    public CircularDependency(CircularDependency dependency) {
        Dependency = dependency;
    }
}