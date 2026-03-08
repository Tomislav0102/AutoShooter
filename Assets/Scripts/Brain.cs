using UnityEngine;

public class Brain : EventBus
{
    public Transform MyTransform { get; set; }
    public Transform MyTarget { get; set; }
    public float moveSpeed;
    public AnimHub animHub;
    public Rigidbody myRigid;
    public PlayerProjectileBody playerProjectileBody;
    public Health health;
    public ILocomotion locomotion;
    public ICombat combat;

    protected override void Awake()
    {
        base.Awake();
        MyTransform = transform;
        health?.Initialize(this);
        locomotion = GetComponent<ILocomotion>();
        locomotion?.Initialize(this);
        combat = GetComponent<ICombat>();
        combat?.Initialize(this);
        animHub?.Initialize(this);
    }
}
