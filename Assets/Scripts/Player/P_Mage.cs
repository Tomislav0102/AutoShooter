using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
    [ShowInInspector, ReadOnly] SpellHelper _walkTrail, _moveRotate;
    
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            value.loco.lookAtTarget = false;
           // return;
            _walkTrail = Instantiate(Ga.me.spells.walkTrail, Ga.me.spells.myTransform);
            _walkTrail.InitializeMe(value, Ga.me.spells.fireWalk, value.myChar.GetStat(Stats.MagicDamage));
            
            _moveRotate = Instantiate(Ga.me.spells.moveRotate, Ga.me.spells.myTransform);
            MoveRotateScaleSpellHelper moveRotateSpellHelper = _moveRotate as MoveRotateScaleSpellHelper;
            moveRotateSpellHelper.anchor = value.myTransform;
            moveRotateSpellHelper.InitializeMe(value, Ga.me.spells.shieldPlayer, 0);
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        float angle = 180f / (numOfHomingMissiles + 1);
        for (int i = 0; i < numOfHomingMissiles; i++)
        {
            S_Homing spell = Instantiate(Ga.me.spells.homing, Ga.me.spells.myTransform) as S_Homing;
            spell.homingTarget = Br.combat.MyTarget;
            spell.comp.myTransform.position = Br.myTransform.position;
            spell.comp.myTransform.forward = -Br.myTransform.right;
            spell.comp.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);;
            spell.comp.myMesh.position = new Vector3(spell.comp.myMesh.position.x, spawnPoint.position.y, spell.comp.myMesh.position.z);
            spell.InitializeMe(Br, Br.myChar.GetStat(Stats.MagicDamage));
        }
    }
}
