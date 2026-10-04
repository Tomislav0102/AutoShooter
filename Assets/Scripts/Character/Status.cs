using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Status : MonoBehaviour, IIniBrain
{
    public enum Effect
    {
        Invulnerable,
        Rooted, //move speed is 0, attack speed unaffected
        Stunned, //completely passive, enemy does nothing
        Confused, //attacks random character, changes target often, does not respond to aggro 
        Blinded, //like confused, but only close target, movement is roam or idle
        Charmed, //behaves like summon
        Fumbling, //every attack misses
        Burning,
        Freezing,
        Jolted,
        Poisoned,
        Bleeding,
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
        }
    }
    Brain _br;
    MyDuo<BuffEffects, float> _buffTimers;
    float _tickTimer;
    const float CONST_TickMaxTime = 0.5f;
    int _effectsLength;
    MyDuo<Effect, bool> _effectPrevious;
    MyDuo<Effect, bool> _effectActive;
    
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
                    case Effect.Bleeding:
                        break;
                    case Effect.Burning:
                        element = Element.Fire;
                        break;
                    case Effect.Freezing:
                        element = Element.Ice;
                        break;
                    case Effect.Jolted:
                        element = Element.Electricity;
                        break;
                    case Effect.Poisoned:
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
        bool doRefresh = false;
        for (int i = 0; i < buffsToRemove.Count; i++)
        {
            if (_buffTimers.HasKey(buffsToRemove[i]))
            {
                doRefresh = true;
                _buffTimers.RemoveByKey(buffsToRemove[i]);
                break;
            }
        }
        if (doRefresh) Refresh();

    }
    void LateUpdate()
    {
        if (_buffTimers.Length() == 0) return;
       _status.position = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position);
    }
    
    public void StatusInjectData(GenChange change, BuffEffects buffEffects)
    {
        switch (change)
        {
            case GenChange.Add:
                _buffTimers.Add(buffEffects, buffEffects.data.permanent ? float.PositiveInfinity : buffEffects.data.Duration);
                if (buffEffects.IsDot())
                {
                    Refresh();
                    return;
                }

                for (int i = 0; i < _buffTimers.Length() - 1; i++)
                {
                    if (_buffTimers.GetKey(i).effect != buffEffects.effect) continue;
                    float duration = _buffTimers.GetValue(i) +  buffEffects.data.Duration;
                    _buffTimers.SetValue(i, duration);
                    break;
                }

                switch (buffEffects.effect)
                {
                    case Effect.InstantKill:
                        if (Br.health.HealthCurrent <= buffEffects.data.value * Br.character.GetStat(Stats.Health) * 0.01f)
                        {
                            print("Executioner");
                            Br.combat.CombatEventRegistered(CombatEvent.GetHit, buffEffects.myBrain);
                            Br.health.Death(buffEffects.myBrain);
                            return;
                        }
                        break;
                }
                
                break;
            case GenChange.Remove:
                if (_buffTimers.HasKey(buffEffects))
                {
                    _buffTimers.RemoveByKey(buffEffects);
                }
                break;
        }
        
        Refresh();
    }
    void Refresh()
    {
        setEvent();
        uIRefresh();
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
