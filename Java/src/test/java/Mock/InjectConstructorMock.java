package Mock;
import Annotations.InjectConstructor;

/**
 * Created by user on 22/9/2015.
 */
public class InjectConstructorMock {
    public IMockService interfaceService;
    public IMockService service1;
    public IMockService service2;
    public String str ;

    public InjectConstructorMock(IMockService iService, MockService1 service1, MockService2 service2)
    {
        this.interfaceService = iService;
        this.service1 = service1;
        this.service2 = service2;
    }

    @InjectConstructor
    public InjectConstructorMock(MockService1 service1, MockService2 service2)
    {
        this.service1 = service1;
        this.service2 = service2;
    }

    public InjectConstructorMock(IMockService iService, MockService2 service2, String str)
    {
        this.interfaceService = iService;
        this.service2 = service2;
        this.str = str;
    }

}
