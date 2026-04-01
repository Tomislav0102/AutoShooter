using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
    [ShowInInspector, ReadOnly] SpellHelper walkTrail, moveRotate;
    
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            value.loco.lookAtTarget = false;
            if (walkTrail != null) walkTrail.InitializeMe(value, Ga.me.spells.fireWalk);
            if (moveRotate != null)
            {
                MoveRotateScaleSpellHelper moveRotateSpellHelper = moveRotate as MoveRotateScaleSpellHelper;
                moveRotateSpellHelper.anchor = Br.myTransform;
                moveRotateSpellHelper.InitializeMe(value, Ga.me.spells.shieldPlayer);
            }
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
            spell.myTransform.position = Br.myTransform.position;
            spell.myTransform.forward = -Br.myTransform.right;
            spell.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);;
            spell.myMesh.position = new Vector3(spell.myMesh.position.x, spawnPoint.position.y, spell.myMesh.position.z);
            spell.InitializeMe(Br);
        }
    }
}
