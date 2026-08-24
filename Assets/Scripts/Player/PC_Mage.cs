using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PC_Mage : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _playerCombat = GetComponent<PlayerCombat>();
            _numOfObjects = value.character.GetStat(Stats.Projectiles);
            setEngageRange();
            switch (startActive)
            {
                case 0:
                    SpellGroup groupShields = Instantiate(Ga.me.spells.groupOrbitalShields, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
                    orbitalGroupShields.orbitingAnchor = value.myTransform;
                    SpellMain[] shieldsPrefabs = new SpellMain[_numOfObjects];
                    PassDataContainer[] pdShields = new PassDataContainer[_numOfObjects];
                    for (int i = 0; i < _numOfObjects; i++)
                    {
                        shieldsPrefabs[i] = Ga.me.spells.shieldFromProjectiles;
                        pdShields[i] = null;
                    }
                    groupShields.InitializeMe(value, new MyDuo<SpellMain, PassDataContainer>(shieldsPrefabs, pdShields));
                    break;
                case 1:
                    SpellGroup groupWalkTrail = Instantiate(Ga.me.spells.groupWalkTrail, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    PassDataContainer pd = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Fire }, new float[1] { 1f })
                        },
                    };
                    groupWalkTrail.InitializeMe(value, new MyDuo<SpellMain, PassDataContainer>(new SpellMain[1] { Ga.me.spells.walkTrailSingle }, new PassDataContainer[1] { pd }));
                    break;
                case 2:
                    SpellGroup groupSwords = Instantiate(Ga.me.spells.groupOrbitalSwords, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
                    orbitalGroupSwords.orbitingAnchor = value.myTransform;
                    
                    SpellMain[] swords = new SpellMain[_numOfObjects];
                    swords[0] = Ga.me.spells.swordFire;
                    float[] swordsDamage = new float[1] { 0.3f * Br.character.GetStat(Stats.MagicDamage) };
                    PassDataContainer pdFire = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Fire }, swordsDamage)
                        }
                    };
                    PassDataContainer pdIce= new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Ice }, swordsDamage)
                        }
                    };
                    PassDataContainer pdElectricity = new PassDataContainer()
                    {
                        data = new PassData[1]
                        {
                            new PassDataDamage(new Element[1] { Element.Electricity }, swordsDamage)
                        }
                    };
                    PassDataContainer[] pds = new PassDataContainer[_numOfObjects];
                    pds[0] = pdFire;
                    switch (_numOfObjects)
                    {
                        case 2:
                            swords[1] = Ga.me.spells.swordFire;
                            pds[1] = pdFire;    
                            break;
                        case 4:
                            swords[1] = Ga.me.spells.swordIce;
                            swords[2] = Ga.me.spells.swordFire;
                            swords[3] = Ga.me.spells.swordIce;
                            pds[1] = pdIce;    
                            pds[2] = pdFire;    
                            pds[3] = pdIce;    
                            break;
                        case 6:
                            swords[1] = Ga.me.spells.swordIce;
                            swords[2] = Ga.me.spells.swordElectricity;
                            swords[3] = Ga.me.spells.swordFire;
                            swords[4] = Ga.me.spells.swordIce;
                            swords[5] = Ga.me.spells.swordElectricity;
                            pds[1] = pdIce;    
                            pds[2] = pdElectricity;    
                            pds[3] = pdFire;    
                            pds[4] = pdIce;    
                            pds[5] = pdElectricity;    
                            break;
                        default:
                            print("should only be 2, 4, or 6 swords.");
                            return;
                    }
                    groupSwords.InitializeMe(value, new MyDuo<SpellMain, PassDataContainer>(swords, pds));

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
                    float[] anglesY = Utils.RadialSpreadAngles(_numOfObjects, false);
                    for (int i = 0; i < _numOfObjects; i++)
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
                for (int i = 0; i < value.character.skillPair.Length(); i++)
                {
                    SoSkill skill = value.character.skillPair.GetValue(i);
                    if (!skill.hasSpell || skill.skillType != SkillType.Active ) continue;
                    if (value.character.skillPair.GetKey(i) && skill.spell.range > engageRange)
                    {
                        engageRange = skill.spell.range;
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
    int _numOfObjects;

    
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
        for (int i = 0; i < Br.character.skillPair.Length(); i++)
        {
            if (Br.character.skillPair.GetKey(i)) skillActive(i);
        }
        void skillActive(int index)
        {
            SoSkill skill = Br.character.skillPair.GetValue(index);
            switch (index) 
            {
                case 0://homing missile
                    List<Transform> targetsHoming = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                    if (targetsHoming.Count == 0) return;

                    float[] anglesY = Utils.RadialSpreadAngles(_numOfObjects, false);
                    var containerHoming = new PassDataContainer()
                                          {
                                              canBeBlocked = true,
                                              data = new PassData[1]
                                                     {
                                                         new PassDataDamage(new Element[1] { Element.Magic }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                                                     }
                                          };
                    StartCoroutine(rapidStrikesHoming());

                    IEnumerator rapidStrikesHoming()
                    {
                        int counter = 0;
                        for (int i = 0; i < _numOfObjects; i++)
                        {
                            SpellMain homing = Instantiate(skill.spell, Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                            homing.myTransform.rotation *= Quaternion.AngleAxis(anglesY[i], Vector3.up);
                            homing.visual.SetSpawnHeight(spawnPoint.position.y);
                            Transform target = targetsHoming[counter];
                            counter = (1 + counter) % targetsHoming.Count;
                            homing.transporter.target = target;
                            homing.InitializeMe(Br, containerHoming);
                            yield return Ga.me.wait01;
                        }
                    }
                    break;

                case 1: //lightning strike
                    List<Transform> targetsLightStrike = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Furthest, 1, skill.spell.range);
                    Transform furthestTarget = targetsLightStrike.Count == 0 ? null : targetsLightStrike[0];
                    if (furthestTarget == null) return;

                    var containerLightning = new PassDataContainer()
                                             {
                                                 data = new PassData[1]
                                                        {
                                                            new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                                                        }
                                             };
                    SpellMain lightning = Instantiate(skill.spell, furthestTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    lightning.InitializeMe(Br, containerLightning);
                    break;

                case 2: //overload
                    List<Transform> targetsOverload = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                    var containerOverload = new PassDataContainer()
                                            {
                                                data = new PassData[1]
                                                       {
                                                           new PassDataDamage(new Element[1] { Element.Electricity }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                                                       }
                                            };
                    StartCoroutine(rapidStrikesOverload());

                    IEnumerator rapidStrikesOverload()
                    {
                        int counter = 0;
                        for (int i = 0; i < _numOfObjects; i++)
                        {
                            Transform target = targetsOverload[counter];
                            counter = (1 + counter) % targetsOverload.Count;
                            Vector3 distance = target.position - Br.myTransform.position;
                            SpellMain overload = Instantiate(skill.spell, Br.myTransform.position, Quaternion.LookRotation(distance.normalized), Ga.me.spells.myTransform);
                            overload.areaOfEffect = distance.magnitude;
                            overload.transporter.target = target;
                            overload.InitializeMe(Br, containerOverload);
                            yield return Ga.me.wait01;
                        }
                    }
                    break;

                case 3: //carry fireball
                    List<Transform> targetsFireball = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle, 1, skill.spell.range);
                    Transform middleTarget = targetsFireball.Count == 0 ? null : targetsFireball[0];
                    var carrier = new PassDataContainer()
                                  {

                                  };
                    if (middleTarget != null)
                    {
                        Vector3 direction = Utils.Direction(Br.myTransform.position, middleTarget.position);
                        SpellMain carryFireball = Instantiate(skill.spell, Br.myTransform.position, Quaternion.LookRotation(direction), Ga.me.spells.myTransform);
                        carryFireball.InitializeMe(Br, carrier, explosion);
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
                        SpellMain expl = Instantiate(Ga.me.spells.explosionFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                        expl.InitializeMe(Br, containerExplosion, areaFire);
                        Instantiate(Ga.me.psDecalFire, expl.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
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

                case 4: //chain lightning
                    List<Transform> targetsChainLightning = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, _numOfObjects, skill.spell.range);
                    Vector3[] targetPositions = new Vector3[targetsChainLightning.Count];
                    for (int i = 0; i < targetsChainLightning.Count; i++)
                    {
                        targetPositions[i] = targetsChainLightning[i].position;
                    }
                    StartCoroutine(chainLightningCoroutine());

                    IEnumerator chainLightningCoroutine()
                    {
                        for (int i = 0; i < targetsChainLightning.Count; i++)
                        {
                            if (i == 0) chainLightningMethod(Br.myTransform.position, targetPositions[i], i);
                            else chainLightningMethod(targetPositions[i - 1], targetPositions[i], i);
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                        }
                        void chainLightningMethod(Vector3 from, Vector3 to, int index) //index -> every consecutive strike does half damage
                        {
                            Vector3 direction = to - from;
                            SpellMain chainLightning = Instantiate(skill.spell, from, Quaternion.LookRotation(direction.normalized), Ga.me.spells.myTransform);
                            chainLightning.areaOfEffect = direction.magnitude;
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

                case 5:
                    List<Transform> targetsMeteor = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, 1, skill.spell.range);
                    if (targetsMeteor.Count == 0) return;
                    var containerMeteor = new PassDataContainer()
                                          {
                                              data = new PassData[1]
                                                     {
                                                         new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MagicDamage) }),
                                                     }
                                          };
                    SpellMain meteorStrike = Instantiate(skill.spell, targetsMeteor[0].position, Quaternion.identity, Ga.me.spells.myTransform);
                    meteorStrike.InitializeMe(Br, containerMeteor);
                    break;
            }
        }
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
