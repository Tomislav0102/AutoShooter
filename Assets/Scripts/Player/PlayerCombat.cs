using UnityEngine;

public class PlayerCombat : EventBus, ICombat
{
    protected Brain br;
    protected P_Loco myLoco;
    [SerializeField] protected float damage;
    [SerializeField] float specialCooldownTime;
    
    public void Initialize(Brain brain)
    {
        this.br = brain;
        myLoco = br.GetComponent<P_Loco>();
        gm.specialUi.InitializeMe(specialCooldownTime);
    }

    public bool IsReady { get; set; }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.OnSpecialActivated += CallEv_OnSpecialActivated;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.OnSpecialActivated -= CallEv_OnSpecialActivated;
    }
    protected virtual void CallEv_OnSpecialActivated()
    {
        
    }


    [field: SerializeField] public bool IsAttacking { get; set; }
    public Transform MyTarget { get; set; }
    public virtual void AE_Attack(int num = 0) { }

}
