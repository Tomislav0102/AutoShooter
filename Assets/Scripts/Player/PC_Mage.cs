using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// lots of errors with spells that have many projectiles and coroutines for delays between shots (rate of fire)
/// errors appear because enemy is destroyed
/// it will be fixed automatically once pool is implemented
/// </summary>
public class PC_Mage : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _playerCombat = GetComponent<PlayerCombat>();
            _waitForSeconds = Utils.GetWait(0.1f);
            setEngageRange();
            switch (startActive)
            {
                case 0:
                    SpellMain[] shields = new SpellMain[numOfObjects];
                    for (int i = 0; i < numOfObjects; i++)
                    {
                        shields[i] = Instantiate(Ga.me.spells.shieldFromProjectiles, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    }
                    SpellGroup groupShields = Instantiate(Ga.me.spells.groupOrbitalShields, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
                    orbitalGroupShields.orbitingAnchor = value.myTransform;
                    groupShields.InitializeMe(value, shields);
                    break;
                case 1:
                    SpellGroup groupWalkTrail = Instantiate(Ga.me.spells.groupWalkTrail, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    groupWalkTrail.InitializeMe(value, Ga.me.spells.walkTrailSingle);
                    break;
                case 2:
                    SpellGroup groupSwords = Instantiate(Ga.me.spells.groupOrbitalSwords, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                    orbitalGroupSwords.orbitingAnchor = value.myTransform;

                    SpellMain[] swords = new SpellMain[numOfObjects];
                    switch (numOfObjects)
                    {
                        case 1:
                            break;
                        case 2:
                            swords[1] = Instantiate(Ga.me.spells.swordFire, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        case 4:
                            swords[1] = Instantiate(Ga.me.spells.swordIce, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[2] = Instantiate(Ga.me.spells.swordFire, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[3] = Instantiate(Ga.me.spells.swordIce, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        case 6:
                            swords[1] = Instantiate(Ga.me.spells.swordIce, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[2] = Instantiate(Ga.me.spells.swordElectricity, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[3] = Instantiate(Ga.me.spells.swordFire, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[4] = Instantiate(Ga.me.spells.swordIce, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            swords[5] = Instantiate(Ga.me.spells.swordElectricity, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                            break;
                        default:
                            print("should only be 1, 2, 4, or 6 swords.");
                            return;
                    }
                    swords[0] = Instantiate(Ga.me.spells.swordFire, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    groupSwords.InitializeMe(value, swords);
                    break;
                case 3:
                    PassDataContainer container = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Fire },
                                new float[1] { value.character.GetStat(Stats.RangedDamage) })
                        }
                    };
                    float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
                    for (int i = 0; i < numOfObjects; i++)
                    {
                        SpellMain flamethrower = Instantiate(Ga.me.spells.flameThrower, value.myTransform.position,
                            Quaternion.AngleAxis(anglesY[i], Vector3.up), Ga.me.spells.myTransform);
                        flamethrower.transporter.target = value.myTransform;
                        flamethrower.InitializeMe(value, container);
                    }
                    break;
                case 4:
                    SpellMain pushPulse = Instantiate(Ga.me.spells.pushPulsating, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    pushPulse.transporter.target = value.myTransform;
                    pushPulse.InitializeMe(value);
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
                    SpellMain manaShield = Instantiate(Ga.me.spells.manaShield, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    manaShield.transporter.target = value.myTransform;
                    manaShield.InitializeMe(value, containerManaShield);
                    break;
            }

            void setEngageRange()
            {
                float engageRange = 0f;
                for (int i = 0; i < weaponRangePair.Length(); i++)
                {
                    if (weaponRangePair.GetKey(i) && weaponRangePair.GetValue(i) > engageRange)
                    {
                        engageRange = weaponRangePair.GetValue(i);
                    }
                }
                _playerCombat.engageRange = Mathf.CeilToInt(engageRange);
            }
        }
    }
    Brain _br;
    
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem psCast;
    public int startActive;
    Coroutine _arcaneShieldCoroutine;
    float _arcaneShieldWaitDuration = 3f;
    PlayerCombat _playerCombat;
    WaitForSeconds _waitForSeconds;
    [SerializeField] MyDuo<bool, float> weaponRangePair;
    [Title("Debug")]
    public int numOfObjects;

    
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

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null)
    {
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
    
    public void AnimEv_AttackCallback(int num = 0)
    {
        psCast.Play(); 
        // if (weaponRangePair.GetKey(0)) WeaponHomingMissile(weaponRangePair.GetValue(0));
        // if (weaponRangePair.GetKey(1)) WeaponLightning(weaponRangePair.GetValue(1));
        // if (weaponRangePair.GetKey(2)) WeaponOverload(weaponRangePair.GetValue(2));
        // if (weaponRangePair.GetKey(3)) WeaponFireball(weaponRangePair.GetValue(3));
        // if (weaponRangePair.GetKey(4)) WeaponChainLightning(weaponRangePair.GetValue(4));
        // if (weaponRangePair.GetKey(5)) WeaponMeteorStrike(weaponRangePair.GetValue(5));
        // if (weaponRangePair.GetKey(6)) WeaponGravityWell(weaponRangePair.GetValue(6));
    }
    
    // void WeaponHomingMissile(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, numOfObjects, maxRange);
    //     if (foundTargets.Count == 0) return;
    //     
    //     float[] anglesY = Utils.RadialSpreadAngles(numOfObjects, false);
    //     var containerHoming = new PassDataContainer()
    //     {
    //         canBeBlocked = true,
    //         data = new PassData[1]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //         }
    //     };
    //     StartCoroutine(rapidStrikes());
    //
    //     IEnumerator rapidStrikes()
    //     {
    //         int counter = 0;
    //         for (int i = 0; i < numOfObjects; i++)
    //         {
    //             SpellMain homing = Instantiate(Ga.me.spells.homingMissile, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
    //             homing.myTransform.rotation *= Quaternion.AngleAxis(anglesY[i], Vector3.up);
    //             homing.visual.SetSpawnHeight(spawnPoint.position.y);
    //             Transform target =  foundTargets[counter];
    //             counter = (1 + counter) % foundTargets.Count;
    //             homing.transporter.target = target;
    //             homing.InitializeMe(Br, containerHoming);
    //             yield return _waitForSeconds;
    //         }
    //     }
    // }
    //
    // void WeaponLightning(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Furthest, 1, maxRange);
    //     Transform furthestTarget = foundTargets.Count == 0 ? null : foundTargets[0];
    //     if (furthestTarget == null) return;
    //     
    //     var containerLightning = new PassDataContainer()
    //     {
    //         data = new PassData[1]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //         }
    //     };
    //     SpellMain lightning = Instantiate(Ga.me.spells.lightningStrike, furthestTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
    //     lightning.InitializeMe(Br, containerLightning);
    // }
    // void WeaponOverload(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, numOfObjects, maxRange);
    //     var containerOverload= new PassDataContainer()
    //     {
    //         data = new PassData[1]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //         }
    //     };
    //     StartCoroutine(rapidStrikes());
    //
    //     IEnumerator rapidStrikes()
    //     {
    //         int counter = 0;
    //         for (int i = 0; i < numOfObjects; i++)
    //         {
    //             Transform target =  foundTargets[counter];
    //             counter = (1 + counter) % foundTargets.Count;
    //             Vector3 distance = target.position - Br.myTransform.position;
    //             SpellMain overload = Instantiate(Ga.me.spells.overload, Br.myTransform.position, Quaternion.LookRotation(distance.normalized), Ga.me.spells.myTransform);
    //             overload.spell.areaOfEffect = distance.magnitude;
    //             overload.transporter.target = target;
    //             overload.InitializeMe(Br, containerOverload);
    //             yield return _waitForSeconds;
    //         }
    //     }
    // }
    // void WeaponFireball(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle, 1, maxRange);
    //     Transform middleTarget = foundTargets.Count == 0 ? null : foundTargets[0];
    //     var carrier = new PassDataContainer()
    //     {
    //
    //     };
    //     if (middleTarget != null)
    //     {
    //         Vector3 direction = Utils.Direction(Br.myTransform.position, middleTarget.position);
    //         SpellMain carryFireball = Instantiate(Ga.me.spells.carryFireball, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
    //         carryFireball.InitializeMe(Br, carrier, explosion);
    //     }
    //         
    //     void explosion()
    //     {
    //         if (Br.combat.MyTarget == null) return;
    //         var containerExplosion = new PassDataContainer()
    //         {
    //             canBeBlocked = true,
    //             data = new PassData[1]
    //             {
    //                 new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //             }
    //         };
    //         SpellMain expl = Instantiate(Ga.me.spells.explosionFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
    //         expl.InitializeMe(Br, containerExplosion, areaFire);
    //         Instantiate(Ga.me.psDecalFire, expl.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
    //     }
    //         
    //     void areaFire()
    //     {
    //         if (Br.combat.MyTarget == null) return;
    //         var containerArea = new PassDataContainer()
    //         {
    //             data = new PassData[1]
    //             {
    //                 new PassDataDamage(new Element[1] { Element.Fire }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //             }
    //         };
    //         SpellMain areFire = Instantiate(Ga.me.spells.areFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
    //         areFire.InitializeMe(Br, containerArea);
    //     }
    // }
    // void WeaponChainLightning(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, numOfObjects,  maxRange);
    //     Vector3[] targetPositions = new Vector3[foundTargets.Count];
    //     for (int i = 0; i < foundTargets.Count; i++)
    //     {
    //         targetPositions[i] = foundTargets[i].position;
    //     }
    //     StartCoroutine(chainLightningCoroutine());
    //         
    //     IEnumerator chainLightningCoroutine()
    //     {
    //         for (int i = 0; i < foundTargets.Count; i++)
    //         {
    //             if (i == 0) chainLightningMethod(Br.myTransform.position, targetPositions[i], i);
    //             else chainLightningMethod(targetPositions[i - 1], targetPositions[i], i);
    //             yield return new WaitForFixedUpdate();
    //             yield return new WaitForFixedUpdate();
    //         }
    //         void chainLightningMethod(Vector3 from, Vector3 to, int index) //index -> every consecutive strike does half damage
    //         {
    //             Vector3 direction = to - from;
    //             SpellMain chainLightning = Instantiate(Ga.me.spells.chainLightning, from, Quaternion.LookRotation(direction.normalized), Ga.me.spells.myTransform);
    //             chainLightning.spell.areaOfEffect = direction.magnitude;
    //             float dam = Br.character.GetStat(Stats.MagicDamage);
    //             dam /= (index * index + 1);
    //             var container = new PassDataContainer()
    //             {
    //                 data = new PassData[1]
    //                 {
    //                     new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { dam }),
    //                 }
    //             };
    //             // print($"at {index} damage is {dam}");
    //             chainLightning.InitializeMe(Br, container);
    //         }
    //     }
    // }
    // void WeaponMeteorStrike(float maxRange)
    // {
    //     List<Transform> foundTargets = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, 1, maxRange);
    //     if (foundTargets.Count == 0) return;
    //     var containerMeteor = new PassDataContainer()
    //     {
    //         data = new PassData[1]
    //         {
    //             new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
    //         }
    //     };
    //     SpellMain meteorStrike = Instantiate(Ga.me.spells.meteorStrike, foundTargets[0].position, Quaternion.identity, Ga.me.spells.myTransform);
    //     meteorStrike.InitializeMe(Br, containerMeteor);
    // }
    void WeaponGravityWell(float maxRange)
    {
        
    }

    public void AnimEv_UltimateCallback(int num = 0)
    {
        // SpellMain armageddon = Instantiate(Ga.me.spells.armageddon,  Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        // armageddon.transporter.target = Br.myTransform;
        // PassDataContainer container = new PassDataContainer()
        // {
        //     canBeBlocked = false,
        //     data = new PassData[1]
        //     {
        //         new PassDataDamage(new Element[2] { Element.Physical, Element.Fire }, new float[2] { Br.character.GetStat(Stats.MagicDamage), Br.character.GetStat(Stats.MagicDamage) }),
        //     }
        // };
        //
        // armageddon.InitializeMe(Br, container);

    }

}
