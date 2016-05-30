package Mock;

import Annotations.InjectSetter;

/**
 * Created by user on 22/9/2015.
 */
public class InjectPropertyMock {
    private IMockService interfaceService ;
    private MockService1 service1 ;
    private MockService2 service2;

    public IMockService getInterfaceService() {
        return interfaceService;
    }

    public void setInterfaceService(IMockService interfaceService) {
        this.interfaceService = interfaceService;
    }

    public MockService1 getService1() {
        return service1;
    }

    @InjectSetter
    public void setService1(MockService1 service1) {
        this.service1 = service1;
    }

    public MockService2 getService2() {
        return service2;
    }

    @InjectSetter
    public void setService2(MockService2 service2) {
        this.service2 = service2;
    }
}
