using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellTransporter : MonoBehaviour, IIniSpell
{
    public virtual SpellMain Spell { get; set; }

    [ReadOnly] public Transform target;


    protected void SetSpeed(float speed)
    {
        float sp = Spell.spellActive ? speed : 0f;
        Spell.myRigid.linearVelocity = sp * Spell.myTransform.forward;
    }

    public void ReflectProjectile(Brain newBrain, Vector3 newDirection)
    {
        Spell.OwnersBrain = newBrain;
        Spell.myTransform.rotation = Quaternion.LookRotation(newDirection);
        float speed  = Spell.myRigid.linearVelocity.magnitude;
        SetSpeed(speed);
    }

}
