using UnityEngine;

/// <summary>
/// should have 'warningDelay' = 0 in 'Spell'
/// </summary>
public class VisualTransporter : SpellTransporter
{
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            target = value.visual.transform;
            _targetRigid = target.GetComponent<Rigidbody>();
            switch (myType)
            {
                case MyType.Lob:
                    if (_targetRigid == null)
                    {
                        value.MyPhase = SpellMain.Phase.EndStart;
                        print("no rigidbody, terminating.");
                        return;
                    }
                    int y = randomizeYRot ? Random.Range(0, 360) : 0;
                    target.rotation *= Quaternion.Euler(-45f, y, 0);
                    _targetRigid.isKinematic = false;
                    _targetRigid.useGravity = true;
                    _targetRigid.AddForce(7 * Random.Range(1f, 1.4f) * target.forward, ForceMode.VelocityChange);
                    _rndRot = Random.insideUnitSphere.normalized;
                    break;
            
                //used only in meteor strike. Transforms of particles need to be aligned in prefab
                case MyType.Fall:
                    if (randomizeYRot) target.RotateAround(value.myTransform.position, Vector3.up, Random.Range(0f, 360f));
                    value.spellActive = false;
                    _targetRigid.isKinematic = false;
                    _targetRigid.useGravity = false;
                    _targetRigid.linearVelocity = 10f * target.forward;
                    value.visual.PlayDefault();
                    break;
            }
        }
    }

    enum MyType { Lob, Fall }
    [SerializeField] MyType myType;
    [SerializeField] bool randomizeYRot = true;
    Vector3 _rndRot;
    Rigidbody _targetRigid;
    

    
    

    void Update()
    {
        switch (myType)
        {
            case MyType.Lob:
                target.Rotate(_rndRot);
                if (!Ga.me.LevelMan.InsideLevel(target.position))
                {
                    Spell.myTransform.position = new Vector3(target.position.x, 0.2f, target.position.z);
                    Spell.MyPhase = SpellMain.Phase.EndStart;
                }
                break;
            
            case MyType.Fall:
                if (target.position.y < 0.1f)
                {
                    Spell.myTransform.position = new Vector3(Spell.myTransform.position.x, 0f, Spell.myTransform.position.z);
                    Spell.spellActive = true;
                }
                break;
        }
    }

}
