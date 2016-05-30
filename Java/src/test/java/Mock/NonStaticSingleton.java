package Mock;

/**
 * Created by user on 22/9/2015.
 */
public class NonStaticSingleton {
    public  static int nConstructed ;

    public NonStaticSingleton()
    {
        nConstructed++;
    }
}
