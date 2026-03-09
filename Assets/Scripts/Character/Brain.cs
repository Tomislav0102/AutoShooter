using UnityEngine;
using Sirenix.OdinInspector;

public class Brain : EventBus
{
    public Rigidbody myRigid;
    [Title("References")]
    public Health health;
    public Loco loco;
    public Combat combat;

    
    
    protected override void Awake()
    {
        base.Awake();
        health?.Initialize(this);
        loco?.Initialize(this);
        combat?.Initialize(this);
    }
}
