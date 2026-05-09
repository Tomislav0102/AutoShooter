using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
   // [ShowInInspector, ReadOnly] SpellTransporter _walkTrail, _moveRotateShield, _moveRotateSwords;
    public bool[] spellActive;
    
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
               // { Element.Fire, 22f },
                { Element.Physical, 3f },
            };

            SpellControl shield = Instantiate(Ga.me.spells.shieldFromProjectiles, Br.myTransform.position, Quaternion.identity, Br.myTransform);
            OrbitalTransporter orbital = shield.transporter as  OrbitalTransporter;
            orbital.orbitingAnchor = Br.myTransform;
            shield.InitializeMe(Br);
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

        if (spellActive[0])
        {
            SpellControl lightning = Instantiate(Ga.me.spells.lightningStrike, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
            lightning.InitializeMe(Br, damRanged);
        }
        else if (spellActive[1])
        {
            Vector3 direction = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.position);
            SpellControl carryFireball = Instantiate(Ga.me.spells.carryFireball, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
            carryFireball.InitializeMe(Br, null, Explosion);
            
            void Explosion()
            {
                if (Br.combat.MyTarget == null) return;
                SpellControl explosion = Instantiate(Ga.me.spells.explosionFire, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                explosion.InitializeMe(Br, damRanged, AreaFire);
            }
            
            void AreaFire()
            {
                if (Br.combat.MyTarget == null) return;
                SpellControl areFire = Instantiate(Ga.me.spells.areFire, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                areFire.InitializeMe(Br, damRanged);
            }
        }
        else if (spellActive[2])
        {
            float angle = 180f / (numOfHomingMissiles + 1);
            for (int i = 0; i < numOfHomingMissiles; i++)
            {
                SpellControl homing = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
                homing.myTransform.forward = -Br.myTransform.right;
                homing.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);
                homing.myMesh.position = new Vector3(homing.myMesh.position.x, spawnPoint.position.y, homing.myMesh.position.z);
                
                HomingTransporter homingTransporter = homing.transporter as  HomingTransporter;
                homingTransporter.homingTarget = Br.combat.MyTarget;
                homing.InitializeMe(Br, damRanged);
            }
        }
        
    }
}
