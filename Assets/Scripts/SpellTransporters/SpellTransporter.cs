using System.Collections.Generic;
using UnityEngine;

public class SpellTransporter : MonoBehaviour
{
    protected SpellMain main;

    public virtual void InitializeMe(SpellMain spellMain)
    {
        main = spellMain;
        main.spell.InitializeMe(spellMain);
    }
    
    protected void SetSpeed(float speed)
    {
          main.myRigid.linearVelocity = speed * main.myTransform.forward;
    }

    public void ReflectProjectile(Brain newBrain)
    {
        main.OwnersBrain = newBrain;
        float speed  = main.myRigid.linearVelocity.magnitude;
        main.myTransform.Rotate(Vector3.up, 180f);
        SetSpeed(speed);
    }

}
