using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PC_Mage : P_Combat
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
                    PassDataContainer container = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Fire },
                                new float[1] { Br.character.GetStat(Stats.RangedDamage) })
                        }
                    };
                    float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
                    for (int i = 0; i < numOfObjects; i++)
                    {
                        SpellMain flamethrower = Instantiate(Ga.me.spells.flameThrower, Br.myTransform.position,
                            Quaternion.AngleAxis(anglesY[i], Vector3.up), Ga.me.spells.myTransform);
                        flamethrower.transporter.target = Br.myTransform;
                        flamethrower.InitializeMe(Br, container);
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
                    PassDataContainer containerManaShield = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataManaShield(100)
                        }
                    };
                    SpellMain manaShield = Instantiate(Ga.me.spells.manaShield, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    manaShield.transporter.target = Br.myTransform;
                    manaShield.InitializeMe(Br, containerManaShield); 
                    break;
            }
            
        }
    }

    void ArcaneShieldSpawn()
    {
        PassDataContainer container = new PassDataContainer()
        {
            data = new PassData[1]
            {
                new PassDataSpell(System.Array.Empty<SpellMain>(), PassData.HitEffectOnSpell.Nullify)
            }
        };
        SpellMain arcaneShield = Instantiate(Ga.me.spells.arcaneShield, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        arcaneShield.transporter.target = Br.myTransform;
        arcaneShield.InitializeMe(Br, container);
        arcaneShield.onHitTarget += (Brain br) =>
        {
            ArcaneShieldCoroutineControl();
        };
    }

    void ArcaneShieldCoroutineControl()
    {
        if (_arcaneShieldCoroutine != null) StopCoroutine(_arcaneShieldCoroutine);
        _arcaneShieldCoroutine = StartCoroutine(arcaneShieldWait(_arcaneShieldWaitDuration));
        return;
        
        IEnumerator arcaneShieldWait(float waitTime)
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
        List<Transform> foundTargets;
        switch (attackActive)
        {
            case 0:
                foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Furthest);
                Transform furthestTarget = foundTargets.Count == 0 ? null : foundTargets[0];
                if (furthestTarget != null)
                {
                    var containerLightning = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    SpellMain lightning = Instantiate(Ga.me.spells.lightningStrike, furthestTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    lightning.InitializeMe(Br, containerLightning);
                }
                break;
        
            case 1:
                foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle);
                Transform middleTarget = foundTargets.Count == 0 ? null : foundTargets[0];
                if (middleTarget != null)
                {
                    Vector3 direction = Utils.Direction(Br.myTransform.position, middleTarget.position);
                    SpellMain carryFireball = Instantiate(Ga.me.spells.carryFireball, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
                    carryFireball.InitializeMe(Br, null, explosion);
                }
            
                void explosion()
                {
                    if (Br.combat.MyTarget == null) return;
                    var containerExplosion = new PassDataContainer()
                    {
                        canBeBlocked = true,
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    explosion.InitializeMe(Br, containerExplosion, areaFire);
                    Instantiate(Ga.me.psDecalFire, explosion.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
                }
            
                void areaFire()
                {
                    if (Br.combat.MyTarget == null) return;
                    var containerArea = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Fire }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    SpellMain areFire = Instantiate(Ga.me.spells.areFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    areFire.InitializeMe(Br, containerArea);
                }
                break;
            
            case 2:
                float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
                var containerHoming = new PassDataContainer()
                {
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                    }
                };
                for (int i = 0; i < numOfObjects; i++)
                {
                    SpellMain homing = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                    homing.myTransform.rotation *= Quaternion.AngleAxis(anglesY[i], Vector3.up);
                    homing.visual.SetSpawnHeight(spawnPoint.position.y);
                    homing.transporter.target = Br.combat.MyTarget;
                    homing.InitializeMe(Br, containerHoming);
                }
                break;
            
            case 3:
                foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, numOfObjects);
                Vector3[] targetPositions = new Vector3[foundTargets.Count];
                for (int i = 0; i < foundTargets.Count; i++)
                {
                    targetPositions[i] = foundTargets[i].position;
                }
                StartCoroutine(chainLightningCoroutine());
            
                IEnumerator chainLightningCoroutine()
                {
                    for (int i = 0; i < foundTargets.Count; i++)
                    {
                        if (i == 0) chainLightningMethod(Br.myTransform.position, targetPositions[i], i);
                        else chainLightningMethod(targetPositions[i - 1], targetPositions[i], i);
                        yield return new WaitForFixedUpdate();
                        yield return new WaitForFixedUpdate();
                    }
                    void chainLightningMethod(Vector3 from, Vector3 to, int index) //index -> every consecutive strike does half damage
                    {
                        Vector3 direction = to - from;
                        SpellMain chainLightning = Instantiate(Ga.me.spells.chainLightning, from, Quaternion.LookRotation(direction.normalized), Ga.me.spells.myTransform);
                        chainLightning.spell.areaOfEffect = direction.magnitude;
                        float dam = Br.character.GetStat(Stats.MagicDamage);
                        dam /= (index * index + 1);
                        var container = new PassDataContainer()
                        {
                            data = new PassData[1]
                            {
                                new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { dam }),
                            }
                        };
                       // print($"at {index} damage is {dam}");
                        chainLightning.InitializeMe(Br, container);
                    }
                }
                break;
            
            case 4:
                var containerOverload= new PassDataContainer()
                {
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                    }
                };
                float maxRange = 5f;
                foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, numOfObjects, maxRange);
                for (int i = 0; i < foundTargets.Count; i++)
                {
                    Vector3 distance = foundTargets[i].position - Br.myTransform.position;
                    SpellMain overload = Instantiate(Ga.me.spells.overload, Br.myTransform.position, Quaternion.LookRotation(distance.normalized), Ga.me.spells.myTransform);
                    overload.spell.areaOfEffect = distance.magnitude;
                    overload.transporter.target = foundTargets[i];
                    overload.InitializeMe(Br, containerOverload);
                }
                break;
            
            case 5:
                foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random);
                if (foundTargets.Count > 0)
                {
                    var containerMeteor = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                        }
                    };
                    SpellMain meteorStrike = Instantiate(Ga.me.spells.meteorStrike, foundTargets[0].position, Quaternion.identity, Ga.me.spells.myTransform);
                    meteorStrike.InitializeMe(Br, containerMeteor);
                }
                break;
        }
    }

    public override void FromAnimEv_Ultimate(int num = 0)
    {
        base.FromAnimEv_Ultimate(num);
        SpellMain armageddon = Instantiate(Ga.me.spells.armageddon,  Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        armageddon.transporter.target = Br.myTransform;
        PassDataContainer container = new PassDataContainer()
        {
            canBeBlocked = false,
            data = new PassData[1]
            {
                new PassDataDamage(new Element[2] { Element.Physical, Element.Fire }, new float[2] { Br.character.GetStat(Stats.MagicDamage), Br.character.GetStat(Stats.MagicDamage) }),
            }
        };

        armageddon.InitializeMe(Br, container);

    }
}
