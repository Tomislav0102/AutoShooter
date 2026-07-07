using UnityEngine;

/// <summary>
/// should have 'warningDelay' = 0 in 'Spell'
/// </summary>
public class VisualTransporter : SpellTransporter
{
    enum MyType { Lob, Fall }
    [SerializeField] MyType myType;
    [SerializeField] bool randomizeYRot = true;
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
            
            //used only in meteor strike. Transforms of particles need to be aligned in prefab
            case MyType.Fall:
                if (randomizeYRot) target.RotateAround(main.myTransform.position, Vector3.up, Random.Range(0f, 360f));
                main.IsActive = false;
                _targetRigid.isKinematic = false;
                _targetRigid.useGravity = false;
                _targetRigid.linearVelocity = 10f * target.forward;
                main.visual.PlayDefault();
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
                    main.myTransform.position = new Vector3(target.position.x, 0.2f, target.position.z);
                    main.spell.MyPhase = Spell.Phase.EndStart;
                }
                break;
            
            case MyType.Fall:
                if (target.position.y < 0.1f)
                {
                    main.myTransform.position = new Vector3(main.myTransform.position.x, 0f, main.myTransform.position.z);
                    main.IsActive = true;
                }
                break;
        }
    }

}
