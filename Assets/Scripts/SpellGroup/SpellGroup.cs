using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    public Transform myTransform;
    protected Brain owner;
    protected MyDuo<SpellMain, PassData> prefabsAndData;
    [HideInInspector] public SpellMain[] spellsRunning;
    public GroupPassData groupPassData;
    
    public virtual void InitializeMe(Brain ownersBrain, MyDuo<SpellMain, PassData> duo)
    {
        owner = ownersBrain;
        prefabsAndData = duo;
    }
    public virtual void InitializeMe(Brain ownersBrain, PassData pd)
    {
        owner = ownersBrain;
    }
    
    
    public static void ComboDash(Brain brain, MyDuo<Element, float> damage)
    {
        PassData pdDash = new PassData()
        {
            hasDash =  true,
            dashPower = Ga.me.gameData.dashPower
        };
        SpellMain dash = Instantiate(Ga.me.spells.dash, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        dash.transporter.target = brain.myTransform;
        dash.lifeTime = Ga.me.gameData.dashTime;
        dash.InitializeMe(brain, pdDash);

        Vector2 knockBackDir2 = Utils.MakeV2(brain.myTransform.forward);
        knockBackDir2.Normalize();
        knockBackDir2 = Utils.RotateV2(knockBackDir2, 45f * (2 * Random.Range(0, 2) - 1));
        int knockBackPower = Mathf.Min(20, brain.character.GetStat(Stats.KnockBack));
        PassData pdContactDamage = new PassData()
        {
            hasDamage = true,
            damagePair = damage,
            hasKnockback = true,
            knockbackPower = knockBackPower,
            knockbackDirection =  knockBackDir2,
        };
        SpellMain contactDam = Instantiate(Ga.me.spells.contactDamage, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        contactDam.transporter.target = brain.myTransform;
        contactDam.lifeTime = Ga.me.gameData.dashTime;
        contactDam.areaOfEffect = 1.3f * brain.size;
        contactDam.InitializeMe(brain, pdContactDamage);

    }
    public static SpellGroup Gr(SpellGroup group, Brain brain, bool brainsRotation = false)
    {
        return Instantiate(group, brain.myTransform.position, brainsRotation ? brain.myTransform.rotation : Quaternion.identity, Ga.me.spells.myTransform);
    }

    public class GroupPassData
    {
        public bool setAreaOfEffect;
        public float areaOfEffect;
        public bool setLifeTime;
        public float lifeTime;
    }
}
