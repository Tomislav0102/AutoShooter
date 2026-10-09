using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour, IIniBrain
{
    public enum Effect
    {
        Invulnerable,
        Invisible, //cant be detected or targeted by the enemy except by accident (standing in trajectory of projectile or AOE)
        Rooted, //move speed is 0, attack speed unaffected
        Stunned, //completely passive, enemy does nothing (maybe special animation)
        Frozen, //same as stunned but animation is paused (maybe same visual)
        Confused, //attacks random character, changes target often, does not respond to aggro 
        Blinded, //like confused, but only close target, movement is roam or idle
        Charmed, //behaves like summon
        Fumbling, //every attack misses
        Burning_dot,
        Chilled_dot,
        Jolted_dot,
        Poisoned_dot,
        Bleeding_dot,
        //on weapons (mostly)
        InstantKill,
        Impact, //stunned enemies take 50% more damage
        Keen, //extra crit chance
        Serrated, //2X damage vs unarmored, 0.5X damage vs armored
        Explosive, //bonus knockback
    }
    public static System.Action<Brain, Effect, bool> OnEffectChange;
    
    RectTransform _status;
    GameObject[] _statusImageGos;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _buffTimers = new MyDuo<BuffEffects, float>();
            _status = Instantiate(Ga.me.uiManager.statusPrefab, Ga.me.uiManager.pointersContainer);
            _statusImageGos = Utils.AllChildrenGameObjects(_status);
            _tickTimer = float.PositiveInfinity;
            _effectPrevious = new MyDuo<Effect, bool>();
            _effectActive = new MyDuo<Effect, bool>();
            _effectsLength = System.Enum.GetNames(typeof(Effect)).Length;
            for (int i = 0; i < _effectsLength; i++)
            {
                _effectPrevious.Add((Effect)i, false);
                _effectActive.Add((Effect)i, false);
            }
            _elementLength = System.Enum.GetNames(typeof(Element)).Length;
            _buffStatsBurn = new BuffStats[_elementLength];
            _buffStatsFrozen = new BuffStats[_elementLength];
            _buffStatsStun = new BuffStats[_elementLength];
        }
    }
    Brain _br;
    bool _removedBuff; //so Refresh() does not call every frame
    MyDuo<BuffEffects, float> _buffTimers;
    float _tickTimer;
    const float CONST_TickMaxTime = 0.5f;
    int _effectsLength;
    int _elementLength;
    MyDuo<Effect, bool> _effectPrevious;
    MyDuo<Effect, bool> _effectActive;
    BuffStats[] _buffStatsBurn;
    BuffStats[] _buffStatsFrozen;
    BuffStats[] _buffStatsStun;
    [SerializeField] Status.Effect[] immuneToEffects;

    void OnEnable()
    {
        Ga.OnBrainAddRemove += CallEvOnBrainAddRemove;
    }
    void OnDisable()
    {
        Ga.OnBrainAddRemove -= CallEvOnBrainAddRemove;
    }
    void CallEvOnBrainAddRemove(Brain brain, GenChange change)
    {
        if (brain != Br) return;
        switch (change)
        {
            case GenChange.Remove:
                for (int i = 0; i < _effectsLength; i++)
                {
                    if (HasEffect((Effect)i))
                    {
                        print("removed");
                        OnEffectChange.Invoke(Br, (Effect)i, false);
                    }
                }
                break;
        }
    }

    void OnDestroy()
    {
      if (_status != null)  Destroy(_status.gameObject);
    }
    void Update()
    {
        if (_buffTimers.Length() == 0) return;
        
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= CONST_TickMaxTime)
        {
            _tickTimer = 0f;
        }

        List<BuffEffects> buffsToRemove = new List<BuffEffects>();
        for (int i = 0; i < _buffTimers.Length(); i++)
        {
            BuffEffects buff = _buffTimers.GetKey(i);
            float duration = _buffTimers.GetValue(i);
            
            duration -= Time.deltaTime;
            _buffTimers.SetValue(i, duration);
            if (duration <= 0f) buffsToRemove.Add(buff);
            else
            {
                if (!buff.IsDot() || !Mathf.Approximately(_tickTimer, 0f)) continue;
                Element element = Element.Physical;
                switch (buff.effect)
                {
                    case Effect.Bleeding_dot:
                        break;
                    case Effect.Burning_dot:
                        element = Element.Fire;
                        break;
                    case Effect.Chilled_dot:
                        element = Element.Ice;
                        break;
                    case Effect.Jolted_dot:
                        element = Element.Electricity;
                        break;
                    case Effect.Poisoned_dot:
                        element = Element.Poison;
                        break;
                }
                PassData passData = new PassData()
                {
                    myBrain = buff.myBrain,
                    hasDamage = true,
                    damagePair = new MyDuo<Element, float>(new Element[1] { element }, new float[1] { buff.data.value })
                };
                Br.health.HealthInjectDataDamage(passData, false, false);
            }
        }
        for (int i = 0; i < buffsToRemove.Count; i++)
        {
            StatusInjectData(GenChange.Remove, buffsToRemove[i]);
        }
        if (_removedBuff) Refresh();

    }
    void LateUpdate()
    {
        if (_buffTimers.Length() == 0) return;
       _status.position = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position);
    }
    
    public void StatusInjectData(GenChange change, BuffEffects buffEffect)
    {
        for (int i = 0; i < immuneToEffects.Length; i++)
        {
            if (immuneToEffects[i] != buffEffect.effect) continue;
            print($"immune to {buffEffect.effect}");
            return;
        }
        switch (change)
        {
            case GenChange.Add:
                if (buffEffect.IsDot() && _buffTimers.HasKey(buffEffect)) return;
                
                _buffTimers.Add(buffEffect, buffEffect.data.permanent ? float.PositiveInfinity : buffEffect.data.Duration);
                for (int i = 0; i < _buffTimers.Length() - 1; i++)
                {
                    if (_buffTimers.GetKey(i).effect != buffEffect.effect) continue;
                    float duration = _buffTimers.GetValue(i) +  buffEffect.data.Duration;
                    _buffTimers.SetValue(i, duration);
                    Refresh();
                    return;
                }
                switch (buffEffect.effect)
                {
                    case Effect.InstantKill:
                        if (Br.health.HealthCurrent <= buffEffect.data.value * Br.character.GetStat(Stats.Health) * 0.01f)
                        {
                            print("Executioner");
                            Br.combat.CombatEventRegistered(CombatEvent.GetHit, buffEffect.myBrain);
                            Br.health.Death(buffEffect.myBrain);
                            return;
                        }
                        break;
                    case Effect.Frozen:
                        Br.loco.SetMaterial(Ga.me.matFrozen);
                        break;
                }
                break;
            
            case GenChange.Remove:
                if (_buffTimers.HasKey(buffEffect))
                {
                    switch (buffEffect.effect)
                    {
                        case Effect.Frozen:
                            Br.loco.SetMaterial(); 
                            break;
                    }
                    _buffTimers.RemoveByKey(buffEffect);
                    _removedBuff = true;
                }
                break;
        }
        Refresh();
        statusChangesStats();

        void statusChangesStats()
        {
            BuffStats[] bs = null;
            switch (buffEffect.effect)
            {
                case Effect.Stunned:
                    bs = _buffStatsStun;
                    break;
                case Effect.Frozen:
                    bs =  _buffStatsFrozen;
                    break;
                case Effect.Burning_dot:
                    bs =  _buffStatsBurn;
                    break;
            }
            if (bs is null) return;
            switch (change)
            {
                case GenChange.Add:
                    for (int i = 0; i < _elementLength; i++)
                    {
                        int val = Ga.me.runData.enEffectVulnerabilities.GetValueByKey(buffEffect.effect)[i];
                        if (buffEffect.effect == Effect.Stunned) print(val);
                        if (val == 0) continue;
                        bs[i] = new BuffStats(Character.StatByElement((Element)i, false), BuffType.Added, val);
                        Br.character.CharacterInjectData(GenChange.Add, bs[i]);
                    }
                    break;
                case GenChange.Remove:
                    for (int i = 0; i < _elementLength; i++)
                    {
                        if (bs[i] is null) continue;
                        Br.character.CharacterInjectData(GenChange.Remove, bs[i]);
                        bs[i] = null;
                    }
                    break;
            }
        }
    }
    void Refresh()
    {
        setEvent();
        uIRefresh();
        _removedBuff = false;
        return;
        void setEvent()
        {
            for (int i = 0; i < _effectsLength; i++)
            {
                _effectPrevious.SetValue(i, _effectActive.GetValue(i));
                _effectActive.SetValue(i, false);
            }
            for (int i = 0; i < _buffTimers.Length(); i++)
            {
                _effectActive.SetValueByKey(_buffTimers.GetKey(i).effect, true);
            }
            for (int i = 0; i < _effectsLength; i++)
            {
                if (_effectPrevious.GetValue(i) == _effectActive.GetValue(i)) continue;
                OnEffectChange.Invoke(Br, _effectActive.GetKey(i), _effectActive.GetValue(i));
            }

        }
        void uIRefresh()
        {
            Utils.ActivateOneArrayElement(_statusImageGos);
            for (int i = 0; i < _buffTimers.Length(); i++)
            {
                _statusImageGos[(int)_buffTimers.GetKey(i).effect].SetActive(true);
            }
        }
    }

    public bool HasEffect(Effect effect)
    {
        for (int i = 0; i < _buffTimers.Length(); i++)
        {
            if (_buffTimers.GetKey(i).effect ==  effect) return true;
        }
        return false;
    }

}
