using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellTransporter : MonoBehaviour
{
    protected SpellMain main;
    [ReadOnly] public Transform target;

    
    public virtual void InitializeMe(SpellMain spellMain)
    {
        main = spellMain;
        main.spell.InitializeMe(spellMain);
    }

    protected void SetSpeed(float speed)
    {
        float sp = main.IsActive ? speed : 0f;
        main.myRigid.linearVelocity = sp * main.myTransform.forward;
    }

    public void ReflectProjectile(Brain newBrain, Vector3 newDirection)
    {
        main.OwnersBrain = newBrain;
        main.myTransform.rotation = Quaternion.LookRotation(newDirection);
        float speed  = main.myRigid.linearVelocity.magnitude;
        SetSpeed(speed);
    }

}
