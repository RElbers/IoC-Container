package Mock;

/**
 * Created by user on 22/9/2015.
 */
public class MultipleDependenciesAndInterface {
    public IMockService interfaceService ;
    public IMockService service1 ;
    public IMockService service2 ;

    public MultipleDependenciesAndInterface(IMockService iService, MockService1 service1, MockService2 service2)
    {
        this.interfaceService = iService;
        this.service1 = service1;
        this.service2 = service2;
    }
}
