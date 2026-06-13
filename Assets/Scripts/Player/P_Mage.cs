using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem psCast;
    public int groupActive;
    public int spellActive;
    [Title("Debug")]
    public int numOfSwords = 1;
    public int numOfHomingMissiles = 1;
    public int numOfFlames = 1;


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
                    groupWalkTrail.myDamage = new Dictionary<Element, float>()
                    {
                        { Element.Fire, 1f }
                    };
                    groupWalkTrail.InitializeMe(Br);
                    break;
                case 2:
                    // float angle = 180f / (numOfSwords);
                    // for (int i = 0; i < numOfSwords; i++)
                    // {
                    //     SpellGroup prefab = null;
                    //     if (i == 0) prefab = Ga.me.spells.groupOrbitalSwordsFire;
                    //     if (i == 1) prefab = Ga.me.spells.groupOrbitalSwordsIce;
                    //     if (i == 2) prefab = Ga.me.spells.groupOrbitalSwordsElectric;
                    //     SpellGroup groupSwords = Instantiate(prefab, Br.myTransform.position, Quaternion.Euler(0f, angle * (i + 1), 0f), Ga.me.spells.myTransform);
                    //     // groupSwords.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);
                    //     OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                    //     orbitalGroupSwords.orbitingAnchor = Br.myTransform;
                    //     groupSwords.InitializeMe(Br);
                    // }
                    break;
                case 3:
                    SpellGroup groupFlamethrower = Instantiate(Ga.me.spells.groupFlamethrowers, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupFlamethrower = groupFlamethrower as OrbitalGroup;
                    orbitalGroupFlamethrower.orbitingAnchor = Br.myTransform;
                    groupFlamethrower.myDamage = new Dictionary<Element, float>()
                    {
                        { Element.Fire, 1f }
                    };
                    groupFlamethrower.InitializeMe(Br);
                    break;
                case 4:
                    SpellGroup homing = Instantiate(Ga.me.spells.groupHomingMissile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalHomingGroup = homing as OrbitalGroup;
                    orbitalHomingGroup.myDamage = damRanged;
                    orbitalHomingGroup.numOfActiveSpells = numOfHomingMissiles;
                    homing.InitializeMe(Br);
                    break;
            }

            // SpellMain pushPulse = Instantiate(Ga.me.spells.pushPulsating, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
            // pushPulse.spell.followTarget = Br.myTransform;
            // pushPulse.InitializeMe(Br);
            // float angleFlamethrower = 180f / (numOfFlames + 1);
            // for (int i = 0; i < numOfFlames; i++)
            // {
            //     SpellMain flamethrower = Instantiate(Ga.me.spells.flameThrower,  Br.myTransform.position, 
            //         Quaternion.Euler(0f, angleFlamethrower * (i + 1) - 90, 0f), Ga.me.spells.myTransform);
            //     flamethrower.spell.followTarget = Br.myTransform;
            //     flamethrower.InitializeMe(Br);
            // }
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        psCast.Play();
        switch (spellActive)
        {
            case 0:
                SpellMain lightning = Instantiate(Ga.me.spells.lightningStrike, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                lightning.InitializeMe(Br, damRanged);
                break;
            case 1:
                Vector3 direction = Utils.Direction(Br.myTransform.position, Br.combat.MyTarget.position);
                SpellMain carryFireball = Instantiate(Ga.me.spells.carryFireball, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
                carryFireball.InitializeMe(Br, null, Explosion);

                void Explosion()
                {
                    if (Br.combat.MyTarget == null) return;
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    explosion.InitializeMe(Br, damRanged, AreaFire);
                    Instantiate(Ga.me.psDecalFire, explosion.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
                }

                void AreaFire()
                {
                    if (Br.combat.MyTarget == null) return;
                    SpellMain areFire = Instantiate(Ga.me.spells.areFire, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    areFire.InitializeMe(Br, damRanged);
                }

                break;
            case 2:
                SpellMain[] spells = new SpellMain[numOfHomingMissiles];
                Transform[] transforms = new Transform[numOfHomingMissiles];
                for (int i = 0; i < numOfHomingMissiles; i++)
                {
                    spells[i] = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    transforms[i] = spells[i].myTransform;
                }
                Utils.RadialSpread(transforms, 0f, false, 180);
                for (int i = 0; i < spells.Length; i++)
                {
                    spells[i].myMesh.position = new Vector3(spells[i].myMesh.position.x, spawnPoint.position.y, spells[i].myMesh.position.z);
                    HomingTransporter transporter = spells[i].transporter as HomingTransporter;
                    transporter.homingTarget = Br.combat.MyTarget;
                    spells[i].InitializeMe(Br, damRanged);
                }
                break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        SpellMain armageddon = Instantiate(Ga.me.spells.armageddon,  Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        armageddon.spell.followTarget = Br.myTransform;
        armageddon.InitializeMe(Br, new Dictionary<Element, float>()
        {
            { Element.Magic, Br.myChar.GetStat(Stats.MagicDamage) * 0.1f },
        });

    }
}
