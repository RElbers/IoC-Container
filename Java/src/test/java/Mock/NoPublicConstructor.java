package Mock;

/**
 * Created by user on 22/9/2015.
 */
public class NoPublicConstructor {
    public MockService1 Service1;

    private NoPublicConstructor(MockService1 service1) {
        Service1 = service1;
    }
}

