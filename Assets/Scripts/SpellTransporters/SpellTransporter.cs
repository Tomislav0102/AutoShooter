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

}
