package Mock;

import Annotations.InjectConstructor;
import Annotations.InjectSetter;

class CircularSetter2
{
    private CircularSetter1 circularSetter1;

    @InjectSetter
    public CircularSetter1 getCircularSetter1(){
        return circularSetter1;
    }

}
 class CircularSetter1
{
    private CircularSetter2 circularSetter2;

    @InjectSetter
    public CircularSetter2 getCircularSetter2(){
        return circularSetter2;
    }
}
public class CircularSetter
{
    private CircularSetter1 circularSetter1;
    private CircularSetter2 circularSetter2;

    @InjectSetter
    public CircularSetter1 getCircularSetter1(){
        return circularSetter1;
    }

    @InjectSetter
    public CircularSetter2 getCircularSetter2(){
        return circularSetter2;
    }
}
