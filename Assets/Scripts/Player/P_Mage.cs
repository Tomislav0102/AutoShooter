using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem psCast;
    public int startActive;
    public int attackActive;
    Coroutine _arcaneShieldCoroutine;
    float _arcaneShieldWaitDuration = 3f;
    [Title("Debug")]
    public int numOfObjects;


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
                // { Element.Electricity, 1f },
                { Element.Fire, 1f },
                // { Element.Physical, 3f },
            };
            damUltimate = new Dictionary<Element, float>()
            {
                { Element.Magic, Br.myChar.GetStat(Stats.MagicDamage) * 0.1f },
            };

            switch (startActive)
            {
                case 0:
                    SpellMain[] shields = new SpellMain[numOfObjects];
                    for (int i = 0; i < numOfObjects; i++)
                    {
                        shields[i] = Instantiate(Ga.me.spells.shieldFromProjectiles, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    }
                    SpellGroup groupShields = Instantiate(Ga.me.spells.groupOrbitalShields, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
                    orbitalGroupShields.orbitingAnchor = Br.myTransform;
                    groupShields.InitializeMe(Br, shields);
                    break;
                case 1:
                    SpellGroup groupWalkTrail = Instantiate(Ga.me.spells.groupWalkTrail, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    groupWalkTrail.InitializeMe(Br, Ga.me.spells.walkTrailSingle);
                    break;
                case 2:
                    SpellGroup groupSwords = Instantiate(Ga.me.spells.groupOrbitalSwords, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                    orbitalGroupSwords.orbitingAnchor = Br.myTransform;
                    
                    SpellMain[] swords = new SpellMain[numOfObjects];
                    switch (numOfObjects)
                    {
                        case 1:
                            break;
                        case 2:
                            swords[1] = Instantiate(Ga.me.spells.swordFire, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        case 4:
                            swords[1] = Instantiate(Ga.me.spells.swordIce, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[2] = Instantiate(Ga.me.spells.swordFire, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[3] = Instantiate(Ga.me.spells.swordIce, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        case 6:
                            swords[1] = Instantiate(Ga.me.spells.swordIce, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[2] = Instantiate(Ga.me.spells.swordElectricity, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[3] = Instantiate(Ga.me.spells.swordFire, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[4] = Instantiate(Ga.me.spells.swordIce, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[5] = Instantiate(Ga.me.spells.swordElectricity, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        default:
                            print("should only be 1, 2, 4, or 6 swords.");
                            return;
                    }
                    swords[0] = Instantiate(Ga.me.spells.swordFire, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    groupSwords.InitializeMe(Br, swords);
                    break;
                case 3:
                    float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
                    for (int i = 0; i < numOfObjects; i++)
                    {
                        SpellMain flamethrower = Instantiate(Ga.me.spells.flameThrower, Br.myTransform.position,
                            Quaternion.AngleAxis(anglesY[i], Vector3.up), Ga.me.spells.myTransform);
                        flamethrower.transporter.target = Br.myTransform;
                        injectHealth = new InjectHealth(damRanged);
                        flamethrower.InitializeMe(Br, injectHealth);
                    }
                    break;
                case 4:
                    SpellMain pushPulse = Instantiate(Ga.me.spells.pushPulsating, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform); 
                    pushPulse.transporter.target = Br.myTransform;
                    pushPulse.InitializeMe(Br);
                    break;
                case 5:
                    ArcaneShieldSpawn();
                    break;
                case 6:
                    SpellMain manaShield = Instantiate(Ga.me.spells.manaShield, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    manaShield.transporter.target = Br.myTransform;
                    manaShield.InitializeMe(Br); //values are defined in inspector
                    break;
            }

        }
    }

    void ArcaneShieldSpawn()
    {
        SpellMain arcaneShield = Instantiate(Ga.me.spells.arcaneShield, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        arcaneShield.transporter.target = Br.myTransform;
        arcaneShield.InitializeMe(Br);
        arcaneShield.onHitTarget += (Brain br) =>
        {
            ArcaneShieldCoroutineControl();
        };
    }

    void ArcaneShieldCoroutineControl()
    {
        if (_arcaneShieldCoroutine != null) StopCoroutine(_arcaneShieldCoroutine);
        _arcaneShieldCoroutine = StartCoroutine(ArcaneShieldWait(_arcaneShieldWaitDuration));
        return;
        
        IEnumerator ArcaneShieldWait(float waitTime)
        {
            yield return  new WaitForSeconds(waitTime);
            ArcaneShieldSpawn();
        }

    }


    public override void CombatEventRegistered(CombatEvent combatEvent, Brain otherBrain = null)
    {
        base.CombatEventRegistered(combatEvent, otherBrain);
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                break;
            case CombatEvent.Hit:
                break;
            case CombatEvent.Miss:
                break;
            case CombatEvent.GetHit:
                if (startActive == 5) //arcane shield
                {
                    ArcaneShieldCoroutineControl();
                }
                break;
            case CombatEvent.Block:
                break;
            case CombatEvent.Kill:
                break;
        }
    }

    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        psCast.Play();
        switch (attackActive)
        {
            case 0:
                SpellMain lightning = Instantiate(Ga.me.spells.lightningStrike, Br.combat.MyTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                injectHealth = new InjectHealth(damRanged);
                lightning.InitializeMe(Br, injectHealth);
                break;
            
            case 1:
                Transform middleTarget = Utils.ChoseTransform(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle);
                Vector3 direction = Utils.Direction(Br.myTransform.position, middleTarget.position);
                SpellMain carryFireball = Instantiate(Ga.me.spells.carryFireball, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
                carryFireball.InitializeMe(Br, null, Explosion);

                void Explosion()
                {
                    if (Br.combat.MyTarget == null) return;
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    injectHealth = new InjectHealth(damRanged);
                    explosion.InitializeMe(Br, injectHealth, AreaFire);
                    Instantiate(Ga.me.psDecalFire, explosion.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
                }

                void AreaFire()
                {
                    if (Br.combat.MyTarget == null) return;
                    SpellMain areFire = Instantiate(Ga.me.spells.areFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    injectHealth = new InjectHealth(damRanged);
                    areFire.InitializeMe(Br, injectHealth);
                }
                break;
            
            case 2:
                float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
                for (int i = 0; i < numOfObjects; i++)
                {
                    SpellMain homing = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                    homing.myTransform.rotation *= Quaternion.AngleAxis(anglesY[i], Vector3.up);
                    homing.visual.SetSpawnHeight(spawnPoint.position.y);
                    homing.transporter.target = Br.combat.MyTarget;
                    injectHealth = new InjectHealth(damRanged, true);
                    homing.InitializeMe(Br, injectHealth);
                }
                break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        SpellMain armageddon = Instantiate(Ga.me.spells.armageddon,  Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        armageddon.transporter.target = Br.myTransform;
        injectHealth = new InjectHealth(damUltimate);
        armageddon.InitializeMe(Br, injectHealth);

    }
}
