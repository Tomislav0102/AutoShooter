using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
            value.skills.onSkillIncrease += SkillIncreaseCallback;
        }
    }
    Brain _br;
    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem psCast;
    PlayerCombat _playerCombat;
    int _numOfObjects;
    bool _isCasting; //debug. all attacks must finish before new animation event triggers a cast coroutine

    #region SKILLS
    List<SoSkill> _activeSkills = new List<SoSkill>();
    SoSkill _skillArcaneShield;
    SpellMain _spellArcaneShield;
    bool _canArcaneShield;
    Coroutine  _coroutineArcaneShield;
    SpellMain _spellManaShield;
    SpellMain _spellBastionPulse;
    SpellMain _spellDragonsBreath;
    int _pyromaniaBonus;
    BuffStats _buffPyromania;
    SoSkill _skillChillingTouch;
    HashSet<Brain> _chilledBrains;
    BuffStats[] _buffsChillingTouch;
    SoSkill _skillGlacialShield;
    SpellMain _spellGlacialShield;
    bool _canGlacialShield;
    #endregion

    
    
    void OnEnable()
    {
        Status.OnEffectChange += CallEvStatusEffects;
    }
    void OnDisable()
    {
        Br.skills.onSkillIncrease -= SkillIncreaseCallback;
        Status.OnEffectChange -= CallEvStatusEffects;
    }
    void CallEvStatusEffects(Brain brain, Status.Effect effect, bool on)
    {
        if (brain == Br || brain.Faction != Faction.BadGuys) return;
        if (_pyromaniaBonus == 0)
        {
            if (_buffPyromania == null) return;
            Br.character.CharacterInjectData(GenChange.Remove, _buffPyromania);
            _buffPyromania = null;
            return;
        }
        switch (effect)
        {
            case Status.Effect.Burning_dot:
                if (_buffPyromania != null) Br.character.CharacterInjectData(GenChange.Remove, _buffPyromania);
                if (on)
                {
                    _buffPyromania = new BuffStats(Stats.DamPhysical, BuffType.Percentage, _pyromaniaBonus * Ga.me.runData.enUnderEffect.GetValueByKey(Status.Effect.Burning_dot));
                    Br.character.CharacterInjectData(GenChange.Add, _buffPyromania);
                }
                break;
        }
    }


    void SkillIncreaseCallback(SoSkill newSkill)
    {
        if (newSkill.skillType == SkillType.Active) _activeSkills.Add(newSkill);
        setEngageRange();
        switch (newSkill.skillName)
        {
            case SkillName.ArcaneShield:
                if (_spellArcaneShield is not null) _spellArcaneShield.MyPhase = SpellMain.Phase.EndEnd;
                _skillArcaneShield = newSkill;
                _spellArcaneShield = SpellMain.Sp(_skillArcaneShield.spell, Br);
                _spellArcaneShield.transporter.target = Br.myTransform;
                _spellArcaneShield.InitializeMe(Br);
                _canArcaneShield = true;
                Br.status.StatusInjectData(GenChange.Add, _skillArcaneShield.passData.effects[0]);
                break;
            case SkillName.ManaShield:
                if (_spellManaShield is not null) _spellManaShield.MyPhase = SpellMain.Phase.EndEnd;
                Br.health.HealthInjectDataManaShield(newSkill.passData.manaShieldPoints);
                _spellManaShield = SpellMain.Sp(newSkill.spell, Br);
                _spellManaShield.transporter.target = Br.myTransform;
                _spellManaShield.InitializeMe(Br);
                break;
            case SkillName.BastionPulse:
                if (_spellBastionPulse is not  null) _spellBastionPulse.MyPhase = SpellMain.Phase.EndEnd;
                _spellBastionPulse = SpellMain.Sp(newSkill.spell, Br);
                _spellBastionPulse.transporter.target = Br.myTransform;
                _spellBastionPulse.pd = newSkill.passData;
                _spellBastionPulse.areaOfEffect += newSkill.passData.stats[0].data.value;
                _spellBastionPulse.InitializeMe(Br);
                break;
            case SkillName.DragonsBreath:
                if (_spellDragonsBreath is not null) _spellDragonsBreath.MyPhase = SpellMain.Phase.EndEnd;
                _spellDragonsBreath = SpellMain.Sp(newSkill.spell, Br);
                _spellDragonsBreath.transporter.target = Br.myTransform;
                _spellDragonsBreath.areaOfEffect += newSkill.passData.stats[0].data.value;
                PassData pd = new PassData()
                {
                    hasDamage = true,
                    damagePair = Br.character.GetDamage(Element.Fire, 0.1f, newSkill.passData.damagePair),
                    hasKnockback = true,
                    knockbackPower = newSkill.passData.knockbackPower,
                };
                _spellDragonsBreath.InitializeMe(Br, pd);
                break;
            case SkillName.BlazeTrail:
                SpellGroup groupWalkTrail = SpellGroup.Gr(newSkill.spellGroup, Br);
                PassData pdWalkTrail = new PassData()
                {
                    myBrain = Br,
                    hasDamage = true,
                    damagePair = Br.character.GetDamage(Element.Fire)
                };
                groupWalkTrail.groupPassData = new SpellGroup.GroupPassData()
                {
                    setAreaOfEffect = true,
                    areaOfEffect = 1 + Br.character.GetStat(Stats.Size) * 0.1f,
                    setLifeTime = true,
                    lifeTime = newSkill.valueGeneric + Br.character.GetStat(Stats.Duration) * 0.1f,
                };
                groupWalkTrail.InitializeMe(Br, new MyDuo<SpellMain, PassData>(new SpellMain[1] { Ga.me.spells.walkTrailSingle }, new PassData[1] { pdWalkTrail }));
                break;
            case SkillName.Meltdown:
                Ga.me.runData.enBurnArmorReduction.ChangeBuff(GenChange.Add, BuffType.Percentage, (int)newSkill.valueGeneric);
                break;
            case SkillName.Pyromania:
                if (_buffPyromania != null) Br.character.CharacterInjectData(GenChange.Remove, _buffPyromania);
                _pyromaniaBonus = 2 * (newSkill.level + 1);
                break;
            case SkillName.ChillingTouch:
                _skillChillingTouch = newSkill;
                if (newSkill.level > 0)
                {
                    foreach (Brain item in _chilledBrains)
                    {
                        for (int i = 0; i < _buffsChillingTouch.Length; i++)
                        {
                            item.character.CharacterInjectData(GenChange.Remove, _buffsChillingTouch[i]);   
                        }
                    }
                }
                _buffsChillingTouch = newSkill.passData.stats;
                _chilledBrains = new HashSet<Brain>(); 
                break;
            case SkillName.OrbitalBulwark:
                SpellGroup groupShields = SpellGroup.Gr(newSkill.spellGroup, Br);
                OrbitalGroup orbitalGroupShields = groupShields as OrbitalGroup;
                orbitalGroupShields.orbitingAnchor = Br.myTransform;
                SpellMain[] shieldsPrefabs = new SpellMain[_numOfObjects];
                PassData[] pdShields = new PassData[_numOfObjects];
                for (int i = 0; i < _numOfObjects; i++)
                {
                    shieldsPrefabs[i] = Ga.me.spells.shieldFromProjectiles;
                    pdShields[i] = null;
                }
                groupShields.InitializeMe(Br, new MyDuo<SpellMain, PassData>(shieldsPrefabs, pdShields));
                break;
            case SkillName.GlacialShield:
                _skillGlacialShield = newSkill;
                _spellGlacialShield = SpellMain.Sp(_skillGlacialShield.spell, Br);
                _spellGlacialShield.transporter.target = Br.myTransform;
                _spellGlacialShield.InitializeMe(Br);
                _canGlacialShield = true;
                break;
            case SkillName.Armageddon: //ultimate fire
                Br.skills.myUltimate = newSkill;
                Ga.me.uiManager.ultimateUi.SetMeUp(newSkill.valueGeneric);
                break;

        }
        return;
        
        void setEngageRange()
        {
            float engageRange = 0f;
            foreach (SoSkill item in _activeSkills)
            {
                if (item.hasSpell != SoSkill.HasSpell.Spell) continue;
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
                chillTouch();
                glacialShield();
                void chillTouch()
                {
                    if (_skillChillingTouch is null) return;
                    if (otherBrain is null) return;
                    if (_chilledBrains.Contains(otherBrain)) return;
                    _chilledBrains.Add(otherBrain);
                    for (int i = 0; i < _buffsChillingTouch.Length; i++)
                    {
                        otherBrain.character.CharacterInjectData(GenChange.Add, _buffsChillingTouch[i]);
                    }
                    SpellMain spell = SpellMain.Sp(_skillChillingTouch.spell, otherBrain);
                    spell.transporter.target = otherBrain.myTransform;
                    spell.InitializeMe(Br);
                }
                void glacialShield()
                {
                    if (_skillGlacialShield is null) return;
                    if (otherBrain is null) return;
                  //  if (specialty !=  SpellMain.Specialty.Melee) return;
                    if (!_canGlacialShield)  return;
                    otherBrain.status.StatusInjectData(GenChange.Add, _skillGlacialShield.passData.effects[0]);
                    _spellGlacialShield.visual.StopDefault();
                    _canGlacialShield = false;
                    IEnumerator glacialShieldWait()
                    {
                        yield return new WaitForSeconds(_skillGlacialShield.valueGeneric);
                        _canGlacialShield = true;
                        _spellGlacialShield.visual.PlayDefault();
                    }
                    
                }
                break;
            case CombatEvent.Miss:
                break;
            case CombatEvent.BeginGetHit:
                break;
            case CombatEvent.GetHit:
                arcaneShield();
                void arcaneShield()
                {
                    if (_skillArcaneShield == null) return;
                    if (_coroutineArcaneShield != null) StopCoroutine(_coroutineArcaneShield);
                    _coroutineArcaneShield = StartCoroutine(arcaneShieldWait());
                    if (!_canArcaneShield) return;
                    Br.status.StatusInjectData(GenChange.Remove, _skillArcaneShield.passData.effects[0]);
                    _spellArcaneShield.visual.StopDefault();
                    _canArcaneShield = false;
                    return;
        
                    IEnumerator arcaneShieldWait()
                    {
                        yield return  new WaitForSeconds(_skillArcaneShield.valueGeneric);
                        Br.status.StatusInjectData(GenChange.Add, _skillArcaneShield.passData.effects[0]);
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
        if (_isCasting)
        {
            Debug.LogError("Still casting! Coroutine is taking too long or attack (animation) speed is too fast, fix needed!");
            return;
        }
        psCast.Play();
        StartCoroutine(castSequence());

        IEnumerator castSequence()
        {
            _isCasting = true;
            foreach (SoSkill skill in _activeSkills)
            {
                switch (skill.skillName)
                {
                    case SkillName.MageBase:
                        List<Brain> targetsHoming = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                        if (targetsHoming.Count > 0)
                        {
                            float[] anglesY = Utils.RadialSpreadAngles(_numOfObjects, false);
                            PassData containerHoming = new PassData()
                            {
                                hasDamage = true,
                                damagePair = Br.character.GetDamage(Element.Magic)
                            };
                            int counter = 0;
                            for (int i = 0; i < _numOfObjects; i++)
                            {
                                SpellMain homing = SpellMain.Sp(skill.spell, Utils.LevelV3(spawnPoint.position), Br.myTransform.rotation);
                                homing.myTransform.rotation *= Quaternion.AngleAxis(anglesY[i], Vector3.up);
                                homing.visual.SetSpawnHeight(spawnPoint.position.y, 1f);
                                Transform target = targetsHoming[counter].myTransform;
                                counter = (1 + counter) % targetsHoming.Count;
                                homing.transporter.target = target;
                                homing.InitializeMe(Br, containerHoming);
                                yield return Ga.me.wait01;
                            }
                        }
                        break;
                    case SkillName.Fireball:
                        List<Brain> targetsFireball = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Middle, 1, skill.spell.range);
                        Brain middleTarget = targetsFireball.Count == 0 ? null : targetsFireball[0];
                        if (middleTarget is not null)
                        {
                            Vector3 targetPosition = middleTarget.myTransform.position;
                            Vector3 direction = Utils.Direction(Br.myTransform.position, targetPosition);
                            SpellMain carryFireball = SpellMain.Sp(skill.spell, Br.myTransform.position, Quaternion.LookRotation(direction));
                            carryFireball.InitializeMe(Br, () => explosion(targetPosition));
                        }

                        void explosion(Vector3 pos)
                        {
                            PassData containerExplosion = new PassData()
                            {
                                hasDamage = true,
                                damagePair = Br.character.GetDamage()
                            };
                            SpellMain spell = SpellMain.Sp(skill.afterSpells[0], pos);
                            spell.areaOfEffect += skill.passData.stats[0].data.value;
                            spell.InitializeMe(Br, containerExplosion, () => areaFire(pos));
                            Instantiate(Ga.me.psDecalFire, spell.myTransform.position, Quaternion.Euler(new Vector3(-90, 0, 0)), Ga.me.transform);
                        }

                        void areaFire(Vector3 pos)
                        {
                            PassData containerArea = new PassData()
                            {
                                hasDamage = true,
                                damagePair = Br.character.GetDamage(Element.Fire)
                            };
                            SpellMain spell = SpellMain.Sp(skill.afterSpells[1], pos);
                            spell.areaOfEffect += skill.passData.stats[0].data.value;
                            spell.InitializeMe(Br, containerArea);
                        }
                        break;
                    case SkillName.MeteorStrike:
                        int count = Br.character.GetStat(Stats.Projectiles) + skill.level;
                        List<Brain> targetsMeteor = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, count, skill.spell.range);
                        if (targetsMeteor.Count > 0)
                        {
                            int targetCounter = 0;
                            for (int i = 0; i < count; i++)
                            {
                                PassData containerMeteor = new PassData()
                                {
                                    hasDamage = true,
                                    damagePair = Br.character.GetDamage(Element.Physical, 1f, skill.passData.damagePair),
                                    hasKnockback = true,
                                    knockbackPower = skill.passData.knockbackPower,
                                    knockbackDirection = Random.insideUnitSphere.normalized
                                };
                                SpellMain meteorStrike = SpellMain.Sp(skill.spell, targetsMeteor[targetCounter]);
                                targetCounter = (1 + targetCounter) % targetsMeteor.Count;
                                meteorStrike.InitializeMe(Br, containerMeteor);
                                yield return Ga.me.wait01;
                            }
                        }
                        break;
                    case SkillName.FireNova:
                        BuffEffects burn = skill.passData.effects[0];
                        burn.myBrain = Br;
                        PassData pdFireNova = new PassData()
                        {
                            hasKnockback = true,
                            knockbackPower = skill.passData.knockbackPower + Br.character.GetStat(Stats.KnockBack),
                            hasDamage = true,
                            damagePair = Br.character.GetDamage(Element.Physical, 1f, skill.passData.damagePair),
                            hasEffect = true,
                            effects = new BuffEffects[1] {burn}
                        };
                        SpellMain spellFireNova = SpellMain.Sp(skill.spell, Br);
                        spellFireNova.areaOfEffect += skill.passData.stats[0].data.value;
                        spellFireNova.InitializeMe(Br, pdFireNova);
                        break;
                    case SkillName.ChainLightning:
                        List<Brain> targetsChainLightning = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Random, _numOfObjects, skill.spell.range);
                        Vector3[] targetPositions = new Vector3[targetsChainLightning.Count];
                        for (int i = 0; i < targetsChainLightning.Count; i++)
                        {
                            targetPositions[i] = targetsChainLightning[i].myTransform.position;
                        }
                        for (int i = 0; i < targetsChainLightning.Count; i++)
                        {
                            chainLightningMethod(i == 0 ? Br.myTransform.position : targetPositions[i - 1], targetPositions[i], i);
                            yield return new WaitForFixedUpdate();
                            yield return new WaitForFixedUpdate();
                        }

                        void chainLightningMethod(Vector3 from, Vector3 to, int index) //index -> every consecutive strike does half damage
                        {
                            Vector3 direction = to - from;
                            SpellMain chainLightning = SpellMain.Sp(skill.spell, from, Quaternion.LookRotation(direction.normalized));
                            chainLightning.areaOfEffect = direction.magnitude;
                            float dam = Br.character.GetStat(Stats.DamElectricity);
                            dam /= (index * index + 1);
                            PassData container = new PassData()
                            {
                                hasDamage = true,
                                damagePair = new MyDuo<Element, float>(new Element[1] { Element.Electricity }, new float[1] { dam })
                            };
                            // print($"at {index} damage is {dam}");
                            chainLightning.InitializeMe(Br, container);
                        }
                        break;
                    case SkillName.Overload:
                        List<Brain> targetsOverload = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Closest, _numOfObjects, skill.spell.range);
                        PassData containerOverload = new PassData()
                        {
                            hasDamage = true,
                            damagePair = Br.character.GetDamage(Element.Electricity)
                        };
                        int counterOverload = 0;
                        for (int i = 0; i < _numOfObjects; i++)
                        {
                            Brain target = targetsOverload[counterOverload];
                            counterOverload = (1 + counterOverload) % targetsOverload.Count;
                            Vector3 distance = target.myTransform.position - Br.myTransform.position;
                            SpellMain overload = SpellMain.Sp(skill.spell, Br.myTransform.position, Quaternion.LookRotation(distance.normalized));
                            overload.areaOfEffect = distance.magnitude;
                            overload.transporter.target = target.myTransform;
                            overload.InitializeMe(Br, containerOverload);
                            yield return Ga.me.wait01;
                        }
                        break;
                    case SkillName.LightningBolt:
                        List<Brain> targetsLightStrike = Utils.ChooseGroupTransforms(Br.myTransform.position, Ga.me.team.ValidTargets(Br.Faction), GenDistance.Furthest, 1, skill.spell.range);
                        Brain furthestTarget = targetsLightStrike.Count == 0 ? null : targetsLightStrike[0];
                        if (furthestTarget is not null)
                        {
                            PassData containerLightning = new PassData()
                            {
                                hasDamage = true,
                                damagePair = Br.character.GetDamage(Element.Electricity)
                            };
                            SpellMain lightning = SpellMain.Sp(skill.spell, furthestTarget);
                            lightning.InitializeMe(Br, containerLightning);
                        }
                        break;
                    case SkillName.ShardWave:
                        BuffEffects effectShard = skill.passData.effects[0];
                        effectShard.myBrain = Br;
                        PassData containerShardWave = new PassData()
                        {
                            hasDamage = true,
                            damagePair = Br.character.GetDamage(new Element[2] { Element.Physical, Element.Ice }, 1f, skill.passData.damagePair),
                            hasEffect = true,
                            effects = new BuffEffects[1] { effectShard }
                        };
                        SpellMain spellShardWave = SpellMain.Sp(skill.spell, Br, true);
                        spellShardWave.InitializeMe(Br, containerShardWave);
                        break;
                    case SkillName.IceSpear:
                        if (Br.combat.MyTarget is null) yield break;
                        PassData pdIceSpear = new PassData()
                        {
                            hasDamage = true,
                            damagePair = Br.character.GetDamage(new Element[2] { Element.Physical , Element.Ice }, 1f, skill.passData.damagePair),
                        };
                        SpellMain spellIceSpear = SpellMain.Sp(skill.spell, Utils.LevelV3(spawnPoint.position));
                        Vector3 directionIceSpear = Utils.Direction(spawnPoint.position, Br.combat.MyTarget.myTransform.position);
                        spellIceSpear.myTransform.rotation = Quaternion.LookRotation(directionIceSpear);
                        spellIceSpear.visual.SetSpawnHeight(spawnPoint.position.y, 1.5f);
                        spellIceSpear.transporter.pierce = Br.character.GetStat(Stats.Piercing) + (int)skill.passData.stats[0].data.value;
                        spellIceSpear.InitializeMe(Br, pdIceSpear);
                        break;
                }
            }

            yield return null;
            _isCasting = false;
        }

    }

    
    public void ShieldMonitor(bool shieldOn)
    {
        if (_spellManaShield is null) return;
        if (shieldOn) _spellManaShield.visual.PlayDefault();
        else _spellManaShield.visual.StopDefault();
    }   


    public void AnimEv_UltimateCallback(int num = 0)
    {
        if (Br.skills.myUltimate is null) return;
        switch (Br.skills.myUltimate.skillName)
        {
            case SkillName.Armageddon:
                SpellMain armageddon = SpellMain.Sp(Br.skills.myUltimate.spell, Br);
                armageddon.transporter.target = Br.myTransform;
                PassData container = new PassData()
                {
                    hasDamage = true,
                    damagePair = Br.character.GetDamage(new Element[2] { Element.Physical, Element.Fire }),
                    hasKnockback = true,
                    knockbackPower = Br.character.GetStat(Stats.KnockBack),
                    hasEffect =  true,
                    effects = new BuffEffects[1]
                    {
                        new BuffEffects(Br, Status.Effect.Burning_dot, 1, 5)
                    }
                };
                
                armageddon.InitializeMe(Br, container);
                break;
        }
    }

}
