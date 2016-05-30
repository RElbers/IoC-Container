package Mock;

/**
 * Created by user on 21/9/2015.
 */
public class MultipleDependencies {
    public MockService1 service1 ;
    public MockService2 service2 ;

    public MultipleDependencies(MockService1 service1, MockService2 service2)
    {
        this.service1 = service1;
        this.service2 = service2;
    }
}
