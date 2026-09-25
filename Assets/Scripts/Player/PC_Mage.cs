using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PC_Mage : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _playerCombat = GetComponent<PlayerCombat>();
            _numOfObjects = (int)value.character.GetStat(Stats.Projectiles);
            // switch (startActive)
            // {
            //     case 0:
            //         SpellGroup groupShields = Instantiate(Ga.me.spells.groupOrbitalShields, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
            //         orbitalGroupShields.orbitingAnchor = value.myTransform;
            //         SpellMain[] shieldsPrefabs = new SpellMain[_numOfObjects];
            //         PassData[] pdShields = new PassData[_numOfObjects];
            //         for (int i = 0; i < _numOfObjects; i++)
            //         {
            //             shieldsPrefabs[i] = Ga.me.spells.shieldFromProjectiles;
            //             pdShields[i] = null;
            //         }
            //         groupShields.InitializeMe(value, new MyDuo<SpellMain, PassData>(shieldsPrefabs, pdShields));
            //         break;
            //     case 1:
            //         SpellGroup groupWalkTrail = Instantiate(Ga.me.spells.groupWalkTrail, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         PassData pd = new PassData()
            //         {
            //             myBrain = value,
            //             hasDamage = true,
            //             damagePair = new MyDuo<Element, float>(new Element[1] { Element.Fire }, new float[1] { 1f })
            //         };
            //         groupWalkTrail.InitializeMe(value, new MyDuo<SpellMain, PassData>(new SpellMain[1] { Ga.me.spells.walkTrailSingle }, new PassData[1] { pd }));
            //         break;
            //     case 2:
            //         SpellGroup groupSwords = Instantiate(Ga.me.spells.groupOrbitalSwords, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         OrbitalGroup orbitalGroupSwords = groupSwords as OrbitalGroup;
            //         orbitalGroupSwords.orbitingAnchor = value.myTransform;
            //         
            //         SpellMain[] swords = new SpellMain[_numOfObjects];
            //         swords[0] = Ga.me.spells.swordFire;
            //         float multiplier = 0.3f;
            //         PassData pdFire = new PassData()
            //         {
            //             myBrain = value,
            //             hasDamage = true,
            //             damagePair = Br.character.GetDamage(Element.Fire, multiplier)
            //         };
            //         PassData pdIce= new PassData()
            //         {
            //             myBrain = value,
            //             hasDamage = true,
            //             damagePair = Br.character.GetDamage(Element.Ice, multiplier)
            //         };
            //         PassData pdElectricity = new PassData()
            //         {
            //             myBrain = value,
            //             hasDamage = true,
            //             damagePair = Br.character.GetDamage(Element.Electricity, multiplier)
            //         };
            //         PassData[] pds = new PassData[_numOfObjects];
            //         pds[0] = pdFire;
            //         switch (_numOfObjects)
            //         {
            //             case 2:
            //                 swords[1] = Ga.me.spells.swordFire;
            //                 pds[1] = pdFire;    
            //                 break;
            //             case 4:
            //                 swords[1] = Ga.me.spells.swordIce;
            //                 swords[2] = Ga.me.spells.swordFire;
            //                 swords[3] = Ga.me.spells.swordIce;
            //                 pds[1] = pdIce;    
            //                 pds[2] = pdFire;    
            //                 pds[3] = pdIce;    
            //                 break;
            //             case 6:
            //                 swords[1] = Ga.me.spells.swordIce;
            //                 swords[2] = Ga.me.spells.swordElectricity;
            //                 swords[3] = Ga.me.spells.swordFire;
            //                 swords[4] = Ga.me.spells.swordIce;
            //                 swords[5] = Ga.me.spells.swordElectricity;
            //                 pds[1] = pdIce;    
            //                 pds[2] = pdElectricity;    
            //                 pds[3] = pdFire;    
            //                 pds[4] = pdIce;    
            //                 pds[5] = pdElectricity;    
            //                 break;
            //             default:
            //                 print("should only be 2, 4, or 6 swords.");
            //                 return;
            //         }
            //         groupSwords.InitializeMe(value, new MyDuo<SpellMain, PassData>(swords, pds));
            //
            //         break;
            //     case 3:
            //         PassData container = new PassData()
            //         {
            //             myBrain = value,
            //             hasDamage = true,
            //             damagePair = Br.character.GetDamage(Element.Fire)
            //         };
            //         float[] anglesY = Utils.RadialSpreadAngles(_numOfObjects, false);
            //         for (int i = 0; i < _numOfObjects; i++)
            //         {
            //             SpellMain flamethrower = Instantiate(Ga.me.spells.flameThrower, value.myTransform.position,
            //                 Quaternion.AngleAxis(anglesY[i], Vector3.up), Ga.me.spells.myTransform);
            //             flamethrower.transporter.target = value.myTransform;
            //             flamethrower.InitializeMe(value, container);
            //         }
            //         break;
            //     case 4:
            //         SpellMain pushPulse = Instantiate(Ga.me.spells.pushPulsating, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         pushPulse.transporter.target = value.myTransform;
            //         pushPulse.InitializeMe(value);
            //         break;
            //     case 6:
            //         PassData containerManaShield = new PassData()
            //         {
            //             myBrain = value,
            //             hasManaShield =  true,
            //             manaShieldPoints = 100
            //         };
            //         SpellMain manaShield = Instantiate(Ga.me.spells.manaShield, value.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
            //         manaShield.transporter.target = value.myTransform;
            //         manaShield.InitializeMe(value, containerManaShield);
            //         break;
            // }
        }
    }
    Brain _br;
    
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem psCast;
    SoSkill _skillArcaneShield;
    SpellMain _spellArcaneShield;
    bool _canArcaneShield;
    PlayerCombat _playerCombat;
    int _numOfObjects;
    public List<SoSkill> _allSkills = new List<SoSkill>();
    
    void OnEnable()
    {
        Skills.OnSkillIncrease += SkillIncreaseCallback;
    }
    void OnDisable()
    {
        Skills.OnSkillIncrease -= SkillIncreaseCallback;
    }

    void SkillIncreaseCallback(SoSkill newSkill)
    {
        _allSkills.Add(newSkill);
        setEngageRange();
        switch (newSkill.skillName)
        {
            case SkillName.ArcaneShield:
                if (_spellArcaneShield != null) _spellArcaneShield.MyPhase = SpellMain.Phase.EndEnd;
                _skillArcaneShield = newSkill;
                _spellArcaneShield = Instantiate(_skillArcaneShield.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                _spellArcaneShield.transporter.target = Br.myTransform;
                _spellArcaneShield.InitializeMe(Br);
                _canArcaneShield = true;
                Br.status.StatusInjectData(GenChange.Add, _skillArcaneShield.buffEffect[0]);
                break;
        }
        return;
        
        void setEngageRange()
        {
            float engageRange = 0f;
            foreach (SoSkill item in _allSkills)
            {
                if (item.skillType != SkillType.Active || item.hasSpell != SoSkill.HasSpell.Spell) continue;
                if (item.spell.range > engageRange)  engageRange = item.spell.range;
            }
            _playerCombat.engageRange = Mathf.CeilToInt(engageRange);
        }
    }

    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null, SpellMain.Specialty specialty = SpellMain.Specialty.General)
    {
        switch (combatEvent)
        {
            case CombatEvent.Strike:
                break;
            case CombatEvent.Hit:
                break;
            case CombatEvent.Miss:
                break;
            case CombatEvent.BeginGetHit:
                break;
            case CombatEvent.GetHit:
                arcaneShield();
                void arcaneShield()
                {
                    if (!_canArcaneShield) return;
                    if (_skillArcaneShield == null) return;
                    Br.status.StatusInjectData(GenChange.Remove, _skillArcaneShield.buffEffect[0]);
                    _spellArcaneShield.visual.StopDefault();
                    _canArcaneShield = false;
                    StartCoroutine(arcaneShieldWait());
                    return;
        
                    IEnumerator arcaneShieldWait()
                    {
                        yield return  new WaitForSeconds(_skillArcaneShield.valueGeneric);
                        Br.status.StatusInjectData(GenChange.Add, _skillArcaneShield.buffEffect[0]);
                        _spellArcaneShield.visual.PlayDefault();
                        _canArcaneShield = true;
                    }
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

        foreach (SoSkill skill in _allSkills)
        {
            if (skill.skillType != SkillType.Active) continue;

            switch (skill.skillName)
            {
                case SkillName.MageBase:
                    List<Transform> targetsHoming = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                    if (targetsHoming.Count == 0) return;

                    float[] anglesY = Utils.RadialSpreadAngles(_numOfObjects, false);
                    var containerHoming = new PassData()
                    {
                        myBrain = Br,
                        canBeBlocked = true,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Magic)
                    };
                    StartCoroutine(rapidStrikesHoming());

                    IEnumerator rapidStrikesHoming()
                    {
                        int counter = 0;
                        for (int i = 0; i < _numOfObjects; i++)
                        {
                            SpellMain homing = Instantiate(skill.spell, Utils.LevelV3(spawnPoint.position), Br.myTransform.rotation, Ga.me.spells.myTransform);
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
                case SkillName.Fireball:
                    List<Transform> targetsFireball = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle, 1, skill.spell.range);
                    Transform middleTarget = targetsFireball.Count == 0 ? null : targetsFireball[0];
                    var carrier = new PassData()
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
                        var containerExplosion = new PassData()
                        {
                            myBrain = Br,
                            canBeBlocked = true,
                            hasDamage = true,
                            damagePair = Br.character.GetDamage()
                        };
                        SpellMain expl = Instantiate(Ga.me.spells.explosionFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                        expl.InitializeMe(Br, containerExplosion, areaFire);
                        Instantiate(Ga.me.psDecalFire, expl.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
                    }

                    void areaFire()
                    {
                        if (Br.combat.MyTarget == null) return;
                        var containerArea = new PassData()
                        {
                            myBrain = Br,
                            hasDamage = true,
                            damagePair = Br.character.GetDamage(Element.Fire)
                        };
                        SpellMain areFire = Instantiate(Ga.me.spells.areFire, middleTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                        areFire.InitializeMe(Br, containerArea);
                    }
                    break;
                case SkillName.MeteorStrike:
                    List<Transform> targetsMeteor = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, 1, skill.spell.range);
                    if (targetsMeteor.Count == 0) return;
                    var containerMeteor = new PassData()
                    {
                        myBrain = Br,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Fire)
                    };
                    SpellMain meteorStrike = Instantiate(skill.spell, targetsMeteor[0].position, Quaternion.identity, Ga.me.spells.myTransform);
                    meteorStrike.InitializeMe(Br, containerMeteor);
                    break;
                case SkillName.FireNova:
                    break;
                case SkillName.FrostNova:
                    break;
                case SkillName.IceSpear:
                    break;
                case SkillName.ChainLightning:
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
                            float dam = Br.character.GetStat(Stats.DamElectricity);
                            dam /= (index * index + 1);
                            var container = new PassData()
                            {
                                myBrain = Br,
                                hasDamage = true,
                                damagePair = new MyDuo<Element, float>(new Element[1] { Element.Electricity }, new float[1] { dam })
                            };
                            // print($"at {index} damage is {dam}");
                            chainLightning.InitializeMe(Br, container);
                        }
                    }
                    break;
                case SkillName.Overload:
                    List<Transform> targetsOverload = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                    var containerOverload = new PassData()
                    {
                        myBrain = Br,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Electricity)
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
                case SkillName.LightningBolt:
                    List<Transform> targetsLightStrike = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Furthest, 1, skill.spell.range);
                    Transform furthestTarget = targetsLightStrike.Count == 0 ? null : targetsLightStrike[0];
                    if (furthestTarget == null) return;

                    var containerLightning = new PassData()
                    {
                        myBrain = Br,
                        hasDamage = true,
                        damagePair = Br.character.GetDamage(Element.Electricity)
                    };
                    SpellMain lightning = Instantiate(skill.spell, furthestTarget.position, Quaternion.identity, Ga.me.spells.myTransform);
                    lightning.InitializeMe(Br, containerLightning);
                    break;
                case SkillName.GravityWell:
                    break;
                case SkillName.OrbOfPower:
                    break;
                case SkillName.ManaSingularity:
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
        //     data = new List<PassData>()
        //     {
        //         new PassDataDamage(new Element[2] { Element.Physical, Element.Fire }, new float[2] { Br.character.GetStat(Stats.MagicDamage), Br.character.GetStat(Stats.MagicDamage) }),
        //     }
        // };
        //
        // armageddon.InitializeMe(Br, container);

    }

}
