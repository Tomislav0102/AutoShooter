using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
   // [ShowInInspector, ReadOnly] SpellTransporter _walkTrail, _moveRotateShield, _moveRotateSwords;
    public bool[] activateTransporters;
    
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            value.loco.lookAtTarget = false;
            IsInitialized = true;
            damRanged = new Dictionary<Element, float>()
            {
              //  { Element.Electricity, Br.myChar.GetStat(Stats.MagicDamage) },
                { Element.Electricity, 1f },
            };

            // if (activateTransporters[0])
            // {
            //     _walkTrail = Instantiate(Ga.me.spells.walkTrail, Ga.me.spells.myTransform);
            //     Dictionary<Element, float> damWalkFire = new Dictionary<Element, float>()
            //     {
            //         { Element.Fire, 6f }
            //     };
            //     _walkTrail.InitializeMe(value, new Spell[]{Ga.me.spells.fireWalk}, damWalkFire);
            // }
            //
            // if (activateTransporters[1])
            // {
            //     _moveRotateShield = Instantiate(Ga.me.spells.moveRotate, Ga.me.spells.myTransform);
            //     OrbitalSpellTransporter orbitalShield = _moveRotateShield as OrbitalSpellTransporter;
            //     orbitalShield.anchor = value.myTransform;
            //     Spell[] spells = new Spell[] { Ga.me.spells.shieldPlayer, Ga.me.spells.shieldPlayer, Ga.me.spells.shieldPlayer };
            //     orbitalShield.distanceFromAnchor = 2f;
            //     orbitalShield.InitializeMe(value, spells, null);
            // }
            // if (activateTransporters[2])
            // {
            //     _moveRotateSwords = Instantiate(Ga.me.spells.moveRotate, Ga.me.spells.myTransform);
            //     OrbitalSpellTransporter orbitalSword = _moveRotateSwords as OrbitalSpellTransporter;
            //     orbitalSword.anchor = value.myTransform;
            //     orbitalSword.distanceFromAnchor = 1f;
            //     Spell[] spellsSwords = new Spell[] { Ga.me.spells.swordFire, Ga.me.spells.swordIce, Ga.me.spells.swordEle };
            //     Dictionary<Element, float> damSwords = new Dictionary<Element, float>()
            //     {
            //         { Element.Fire, 1f },
            //         { Element.Ice, 2f },
            //         { Element.Electricity, 3f },
            //     };
            //     orbitalSword.InitializeMe(value, spellsSwords, damSwords);
            // }
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        // Spell lightningStrike = Instantiate(Ga.me.spells.lightningStrike, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
        // lightningStrike.InitializeMe(Br, damRanged);
        // return;
        //
        // float angle = 180f / (numOfHomingMissiles + 1);
        // for (int i = 0; i < numOfHomingMissiles; i++)
        // {
        //     S_Homing spell = Instantiate(Ga.me.spells.homing, Ga.me.spells.myTransform) as S_Homing;
        //     spell.homingTarget = Br.combat.MyTarget;
        //     spell.comp.myTransform.position = Br.myTransform.position;
        //     spell.comp.myTransform.forward = -Br.myTransform.right;
        //     spell.comp.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);;
        //     spell.comp.myMesh.position = new Vector3(spell.comp.myMesh.position.x, spawnPoint.position.y, spell.comp.myMesh.position.z);
        //     spell.InitializeMe(Br, damRanged);
        // }
    }
}
