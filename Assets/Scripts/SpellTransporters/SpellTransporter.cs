using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellTransporter : MonoBehaviour, IIniSpell
{
    public virtual SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
        }
    }
    SpellMain _spell;
    [ReadOnly] public Transform target;
    [BoxGroup, ReadOnly] public int ricochet;
    [BoxGroup, ReadOnly] public int bounce;
    [BoxGroup, ReadOnly] public int pierce;

    public virtual bool CanRicochet(Collider other)
    {
        Ga.me.uiManager.FloatText(Spell.myTransform.position, "Ricochet!", Color.cornflowerBlue, 3f);
        return true;
    }
    public virtual bool CanBounce(Vector3 normal)
    {
        Ga.me.uiManager.FloatText(Spell.myTransform.position, "Bounce!", Color.aquamarine, 3f);
        return true;
    }
    public virtual bool CanPierce()
    {
        Ga.me.uiManager.FloatText(Spell.myTransform.position, "Pierce!", Color.coral, 3f);
        return true;
    }
    public virtual void ReflectProjectile(Brain newBrain)
    {
        Ga.me.uiManager.FloatText(Spell.myTransform.position, "Reflected!", Color.dimGray, 3f);
        
    }

}
