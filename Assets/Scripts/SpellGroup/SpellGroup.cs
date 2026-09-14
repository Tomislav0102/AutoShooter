using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpellGroup : MonoBehaviour
{
    public Transform myTransform;
    protected Brain owner;
    protected MyDuo<SpellMain, PassDataContainer> prefabsAndData;

    
    public virtual void InitializeMe(Brain ownersBrain, MyDuo<SpellMain, PassDataContainer> duo)
    {
        owner = ownersBrain;
        prefabsAndData = duo;
    }
    
    
    public static void ComboDash(Brain brain, MyDuo<Element, float> damage)
    {
        PassDataContainer pdDash = new PassDataContainer()
        {
            data = new List<PassData>()
            {
                new PassDataDash(Ga.me.gameData.dashPower),
            }
        };
        SpellMain dash = Instantiate(Ga.me.spells.dash, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        dash.transporter.target = brain.myTransform;
        dash.lifeTime = Ga.me.gameData.dashTime;
        dash.InitializeMe(brain, pdDash);

        Vector2 knockBackDir2 = Utils.MakeV2(brain.myTransform.forward);
        knockBackDir2.Normalize();
        knockBackDir2 = Utils.RotateV2(knockBackDir2, 45f * (2 * Random.Range(0, 2) - 1));
        int knockBackPower = Mathf.Min(20, brain.character.GetStat(Stats.KnockBack));
        PassDataContainer pdContactDamage = new PassDataContainer()
        {
            data = new List<PassData>()
            {
              //  new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { 4f }),
                new PassDataDamage(damage),
                new PassDataKnockBack(knockBackPower, knockBackDir2)
            }
        };
        SpellMain contactDam = Instantiate(Ga.me.spells.contactDamage, brain.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        contactDam.transporter.target = brain.myTransform;
        contactDam.lifeTime = Ga.me.gameData.dashTime;
        contactDam.areaOfEffect = 1.3f * brain.size;
        contactDam.InitializeMe(brain, pdContactDamage);

    }

}
