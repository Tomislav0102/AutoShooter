using UnityEngine;

public class VisualTransporter : SpellTransporter
{
    enum MyType { Lob, Fall }
    [SerializeField] MyType myType;
    Vector3 _rndRot;
    Rigidbody _targetRigid;
    
    public override void InitializeMe(SpellMain spellMain)
    {
        base.InitializeMe(spellMain);
        target = main.visual.transform;
        _targetRigid = target.GetComponent<Rigidbody>();
        switch (myType)
        {
            case MyType.Lob:
                if (_targetRigid == null)
                {
                    main.spell.MyPhase = Spell.Phase.EndStart;
                    print("no rigidbody, terminating.");
                    return;
                }
                target.rotation *= Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
                _targetRigid.isKinematic = false;
                _targetRigid.useGravity = true;
                _targetRigid.AddForce(10 * Random.Range(1f, 1.4f) * target.forward, ForceMode.VelocityChange);
                _rndRot = Random.insideUnitSphere.normalized;
                break;
            
            case MyType.Fall:
                break;
        }
    }
    
    

    void Update()
    {
        switch (myType)
        {
            case MyType.Lob:
                target.Rotate(_rndRot);
                if (!Ga.me.LevelMan.InsideLevel(target.position))
                {
                    main.myTransform.position = new Vector3(target.position.x, 0f, target.position.z);
                    main.spell.MyPhase = Spell.Phase.EndStart;
                }
                break;
            
            case MyType.Fall:
                if (target.position.y < 0.1f)
                {
                    main.spell.MyPhase = Spell.Phase.EndStart;
                }
                break;
        }
    }

}
