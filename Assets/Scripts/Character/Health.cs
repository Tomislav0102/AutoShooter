using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Health: MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            // _healthBar = Instantiate(Ga.me.healthBarPrefab, Ga.me.barContainer).GetComponent<Image>();
            // _healthBarTransform = _healthBar.transform;
           // _shieldBar = _healthBarTransform.GetChild(0).GetComponent<Image>();
            _numDisplay = Instantiate(Ga.me.uiManager.numDisplayPrefab, Ga.me.uiManager.barContainer).GetComponent<TextMeshProUGUI>();
            _numDisplayTransform = _numDisplay.transform;
            HealthCurrent = value.character.GetStat(Stats.Health);
            _dictPsElements = new Dictionary<Element, ParticleSystem>();
            for (int i = 0; i < psElements.Length; i++)
            {
                _dictPsElements.Add((Element)i, psElements[i]);
            }
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
            _pointer = Instantiate(Ga.me.uiManager.offScreenPointerPrefab, Ga.me.uiManager.pointersContainer);
            _pointerImage = _pointer.GetComponent<Image>();
        }
    }
    Brain _br;

    Vector3 _screenCenter; 
    RectTransform _pointer;
    Image _pointerImage;
  //  Image _healthBar;
  //  Image _shieldBar;
   // Transform _healthBarTransform;
    TextMeshProUGUI _numDisplay;
    Transform _numDisplayTransform;
    Vector3 _offset = new Vector3(0, 2, 0);
    
    #region HP
    
    float _timerRegen;
    public float HealthCurrent
    {
        get => _healthCurrent;
        private set
        {
            _healthCurrent = value;
            float healthMax = Br.character.GetStat(Stats.Health);
            if (_healthCurrent > healthMax)  _healthCurrent = healthMax;
            // _healthBar.color = Color.Lerp(Color.red, Color.green, value / _healthMax);
            // _healthBar.fillAmount = _healthCurrent / _healthMax;
            onHealthChange?.Invoke(_healthCurrent / healthMax);
            UiUpdate();
        }
    }
    float _healthCurrent;
    [ReadOnly] public int life;
    [SerializeField] UnityEvent<float> onHealthChange;
    public bool IsAtFullHealth() => HealthCurrent >= Br.character.GetStat(Stats.Health);
    #endregion

    #region SHIELD
    
    float ShieldCurrent
    {
        get => _shieldCurrent;
        set
        {
            if (_shieldMax <= 0f) return;
            _shieldCurrent = value;
            _shieldCurrent = Mathf.Clamp(_shieldCurrent, 0, _shieldMax);
            UiUpdate();
        }
    }
    [ShowInInspector, ReadOnly] float _shieldCurrent;
    [ShowInInspector, ReadOnly] float _shieldMax;
    const int CONST_ShieldRegenAmount = 10;
    bool _canRegenerateShield = true;
    Coroutine _coroutineShieldRegen;
    [SerializeField] UnityEvent<bool> onShieldActivate;
    bool _shiftShieldActive;
    #endregion

    [Title("Particles")]
    [SerializeField] ParticleSystem[] psElements;
    Dictionary<Element, ParticleSystem> _dictPsElements;
    [SerializeField] ParticleSystem psHeal;
    [Title("Spell defences")]
    [SerializeField] SpellMain[] immuneSpells;
    public bool IsImmuneToSpell(int spellId)
    {
        for (int i = 0; i < immuneSpells.Length; i++)
        {
            if (spellId == immuneSpells[i].id)
            {
                FloatingText ft = Instantiate(Ga.me.uiManager.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.uiManager.floatingContainer);
                ft.SpawnMe("Immune", Color.deepPink);
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (_canRegenerateShield) ShieldCurrent += CONST_ShieldRegenAmount * Time.deltaTime;
        if (ShieldCurrent > 0f)
        {
            if (!_shiftShieldActive) onShieldActivate?.Invoke(true);
            _shiftShieldActive = true;
        }
        else
        {
            if (_shiftShieldActive) onShieldActivate?.Invoke(false);
            _shiftShieldActive = false;
        }
        
        _timerRegen += Time.deltaTime;
        if (_timerRegen < 1f) return;
        _timerRegen = 0f;
        if (!IsAtFullHealth())  HealthCurrent += Br.character.GetStat(Stats.RegenerationRate) * 0.01f;
    }

    void LateUpdate()
    {
        uIDisplay();
        return;
        
        void uIDisplay()
        {
            Vector3 screenPos = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position + _offset);
           // _healthBarTransform.position = screenPos;
            _numDisplayTransform.position = screenPos;
            bool isBehind = Vector3.Dot(Ga.me.camRig.camTransform.forward, Br.myTransform.position - Ga.me.camRig.camTransform.position) < 0;
            if (isBehind)  screenPos = _screenCenter - (screenPos - _screenCenter).normalized * Screen.width;
            int offset = 50;
            bool isOffScreen = screenPos.x > Screen.width + offset || screenPos.x + offset < 0 ||
                               screenPos.y > Screen.height + offset || screenPos.y + offset < 0 ||
                               isBehind;
    
            if (isOffScreen)
            {
                _pointerImage.enabled = true;
    
                screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
                screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
    
                RectTransform canvasRect = _pointer.parent as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);
                _pointer.anchoredPosition = localPos;
    
                Vector3 angleDir = screenPos - _screenCenter;
                if (isBehind) angleDir = _screenCenter - screenPos;
                float angle = Mathf.Atan2(angleDir.y, angleDir.x) * Mathf.Rad2Deg;
                _pointer.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            }
            else
            {
                _pointerImage.enabled = false;
            }
        }
    }

    void UiUpdate()
    {
        string healthDisplay = $"<color=green>{(int)_healthCurrent}/{Br.character.GetStat(Stats.Health)}</color>";
        if (ShieldCurrent > 0)  _numDisplay.text = $"<color=blue>{(int)ShieldCurrent}/{(int)_shieldMax}\n{healthDisplay}";
        else _numDisplay.text = healthDisplay;
    }
    public void HealthInjectDataManaShield(int val)
    {
        _shieldMax = val;
        ShieldCurrent = _shieldMax;
        HealthCurrent = HealthCurrent; //only to update UI
    }
    public void HealthInjectDataDamage(PassData pd, out bool hitDidDamage)
    {
        hitDidDamage = false;
        Br.combat.CombatEventRegistered(CombatEvent.BeginGetHit, pd.myBrain);
        FloatingText ft = Instantiate(Ga.me.uiManager.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.uiManager.floatingContainer);
        if (pd.canBeDodged)
        {
            Br.combat.CheckDodge(out bool dodged, pd.myBrain);
            if (dodged)
            {
                ft.SpawnMe("Dodged!", Color.moccasin);
                return;
            }
        }
        bool blocked = false;
        if (pd.canBeBlocked)
        {
            Br.combat.CheckBlock(out blocked, pd.myBrain);
            if (blocked)
            {
                Br.combat.CombatEventRegistered(CombatEvent.GetHit, pd.myBrain);
                ft.SpawnMe("Blocked!", Color.gold);
            }
        }
        if (!blocked && pd.hasDamage)
        {
            damageCalculation(out hitDidDamage);
            Br.combat.CombatEventRegistered(CombatEvent.GetHit, pd.myBrain);
            void damageCalculation(out bool hit)
            {
                if (Br.status.HasEffect(Status.Effect.Invulnerable))
                {
                    ft.SpawnMe("Invulnerable!", Color.brown);
                    hit = false;
                    return;
                }
                hit = true;

                float totalDamage = 0f;
                MyDuo<Element, float> damageFinal = damageModified(pd.damagePair);
                for (int i = 0; i < damageFinal.Length(); i++)
                {
                    float val = damageFinal.GetValue(i);
                    switch (val)
                    {
                        case < 0: //heal
                            psHeal.Play();
                            break;
                        case > 0:
                        {
                            ParticleSystem ps = _dictPsElements[damageFinal.GetKey(i)];
                            if (ps != null) ps.Play();
                            if (_coroutineShieldRegen != null) StopCoroutine(_coroutineShieldRegen);
                            _coroutineShieldRegen = StartCoroutine(shieldRegen());
                            
                            IEnumerator shieldRegen()
                            {
                                _canRegenerateShield = false;
                                yield return Ga.me.wait100;
                                _canRegenerateShield = true;
                            }
                            break;
                        }
                    }
                    totalDamage += val;
                }
                velocityAddition();

                float shield = ShieldCurrent;
                ShieldCurrent -= totalDamage;
                if (ShieldCurrent <= 0)
                {
                    HealthCurrent -= (totalDamage - shield);
                    if (HealthCurrent <= 0)
                    {
                        Death(pd.myBrain);
                        return;
                    }
                }
                ft.SpawnMe(damageFinal);
                return;

                MyDuo<Element, float> damageModified(MyDuo<Element, float> damageRaw)
                {
                    float enStunMod = (Br.Faction == Faction.BadGuys && Br.status.HasEffect(Status.Effect.Stunned)) ? Ga.me.gameData.enStunDamageModifier : 1f;
                    MyDuo<Element, float> finalPair = new MyDuo<Element, float>();
                    for (int i = 0; i < damageRaw.Length(); i++)
                    {
                        Element el = damageRaw.GetKey(i);
                        float val = damageRaw.GetValue(i) * 
                                    (1 - 0.01f * Br.character.GetStat(Character.StatByElement(el, false)) *
                                        enStunMod);
                        if (val < 0f) val = 0f;
                        finalPair.Add(el, val);
                    }
                    return finalPair;
                }
                void velocityAddition()
                {
                    if (pd.spellsVelocity.Equals(Vector3.zero)) return;

                    Vector2 result = Utils.MakeV2(pd.spellsVelocity) - Utils.MakeV2(Br.agent.velocity);
                    float totalDamageDebug = totalDamage;
                    totalDamage *= result.magnitude;
                    print($"Velocity changed damage from {totalDamageDebug} to {totalDamage}");
                }
            }
        }

        enemyAggro();
        Br.combat.CombatEventRegistered(CombatEvent.EndGetHit, pd.myBrain);
        return;

        void enemyAggro()
        {
            if (pd.myBrain == null) return;
            if (Br.myTransform == Ga.me.team.playerTransform) return;
            if (Br.combat.MyTarget != null) return;
            print("UnderAttack");
            Br.combat.MyTarget = pd.myBrain.myTransform;
        }
    }





    public void Death(Brain brainThatKilledMe)
    {
        if (brainThatKilledMe != null) brainThatKilledMe.combat.CombatEventRegistered(CombatEvent.Kill, Br);

        if (life <= 0)
        {
            Quaternion rot  = Quaternion.LookRotation(Br.myTransform.forward) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            ParticleSystem ps = Instantiate(Ga.me.psDeath, Br.myTransform.position, rot,Ga.me.transform);
            ps.Play();
            Ga.me.team.Death(Br);
           // Destroy(_healthBar.gameObject);
            Destroy(_pointer.gameObject);
            Destroy(Br.gameObject);
            return;
        }
        life--;
        print($"Extra life saved you, {life} lives remaining");
    }

}