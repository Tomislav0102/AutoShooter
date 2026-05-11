using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    public int groupActive;
    public int spellActive;
    [Title("Swords")]
    public int swordCount = 1;
    
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
               // { Element.Fire, 22f },
               // { Element.Physical, 3f },
            };

            switch (groupActive)
            {
                case 0:
                    SpellGroup groupShields = Instantiate(Ga.me.spells.groupOrbitalShields, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
                    orbitalGroupShields.numOfActiveSpells = 3;
                    orbitalGroupShields.orbitingAnchor = Br.myTransform;
                    groupShields.InitializeMe(Br);
                    break;
                case 1:
                    SpellGroup groupWalkTrail = Instantiate(Ga.me.spells.groupWalkTrail, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    WalkTrailGroup walkTrail = groupWalkTrail as WalkTrailGroup;
                    walkTrail.myDamage = new Dictionary<Element, float>()
                    {
                        { Element.Fire, 1f }
                    };
                    groupWalkTrail.InitializeMe(Br);
                    break;
                case 2:
                    float angle = 180f / (swordCount);
                    for (int i = 0; i < swordCount; i++)
                    {
                        SpellGroup prefab = null;
                        if (i == 0) prefab = Ga.me.spells.groupOrbitalSwordsFire;
                        if (i == 1) prefab = Ga.me.spells.groupOrbitalSwordsIce;
                        if (i == 2) prefab = Ga.me.spells.groupOrbitalSwordsElectric;
                        SpellGroup groupSwords = Instantiate(prefab, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                        groupSwords.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);
                        OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                        orbitalGroupSwords.orbitingAnchor = Br.myTransform;
                        groupSwords.InitializeMe(Br);
                    }
                    break;
            }
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        switch (spellActive)
        {
            case 0:
                SpellControl lightning = Instantiate(Ga.me.spells.lightningStrike, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                lightning.InitializeMe(Br, damRanged);
                break;
            case 1:
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
                break;
            case 2:
                int numOfHomingMissiles = 1;
                float angle = 180f / (numOfHomingMissiles + 1);
                for (int i = 0; i < numOfHomingMissiles; i++)
                {
                    SpellControl homing = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
                    homing.myTransform.forward = -Br.myTransform.right;
                    homing.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);
                    homing.myMesh.position = new Vector3(homing.myMesh.position.x, spawnPoint.position.y, homing.myMesh.position.z);
                    HomingTransporter transporter = homing.transporter as  HomingTransporter;
                    transporter.homingTarget = Br.combat.MyTarget;
                    homing.InitializeMe(Br, damRanged);
                }
                break;
        }
    }
}
