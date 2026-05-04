using System.Collections.Generic;
using UnityEngine;

public class SpellTransporter : MonoBehaviour
{
    protected SpellControl main;

    public virtual void InitializeMe(SpellControl spellControl)
    {
        main = spellControl;
    }
    
    protected void SetSpeed(float speed)
    {
          main.myRigid.linearVelocity = speed * main.myTransform.forward;
    }

}
