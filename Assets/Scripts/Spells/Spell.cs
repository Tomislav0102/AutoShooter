
public class Spell : EventBus
{
    public enum Element { Physical, Fire, Ice, Electricity, Poison, Force, Magic }
    public enum Offense { Touch, Ranged }

    public Element element = Element.Physical;

    protected virtual void OnStart()
    {
        
    }
    protected virtual void OnHit()
    {
        
    }
    protected virtual void OnEnd()
    {
        
    }
}
