using System;
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
             float healthMax = value.character.GetStat(Stats.Health);
            // _healthBarTransform = _healthBar.transform;
           // _shieldBar = _healthBarTransform.GetChild(0).GetComponent<Image>();
            _numDisplay = Instantiate(Ga.me.numDisplayPrefab, Ga.me.barContainer).GetComponent<TextMeshProUGUI>();
            _numDisplay.text = $"{value.character.GetStat(Stats.Health)}/{value.character.GetStat(Stats.Health)}";
            _numDisplayTransform = _numDisplay.transform;
            HealthCurrent = healthMax;
            _dictPsElements = new Dictionary<Element, ParticleSystem>();
            for (int i = 0; i < psElements.Length; i++)
            {
                _dictPsElements.Add((Element)i, psElements[i]);
            }
            _screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
            _pointer = Instantiate(Ga.me.offScreenPointerPrefab, Ga.me.parPointers);
            _pointerImage = _pointer.GetComponent<Image>();
        }
    }
    Brain _br;

    Vector3 _screenCenter; 
    RectTransform _pointer;
    Image _pointerImage;
  //  Image _healthBar;
    Image _shieldBar;
   // Transform _healthBarTransform;
    TextMeshProUGUI _numDisplay;
    Transform _numDisplayTransform;
    Vector3 _offset = new Vector3(0, 2, 0);
    float _timerBars;
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
            _numDisplay.text = $"{_healthCurrent}/{healthMax}";

        }
    }
    float _healthCurrent;
    [SerializeField] UnityEvent<float> onHealthChange;
    public bool IsAtFullHealth() => HealthCurrent >= Br.character.GetStat(Stats.Health);
    float ShieldCurrent
    {
        get => _shieldCurrent;
        set
        {
            if (Mathf.Approximately(_shieldMax, 0f)) return;
            _shieldCurrent = value;
            _shieldCurrent = Mathf.Clamp(_shieldCurrent, 0, _shieldMax);
            _shieldBar.fillAmount = _shieldCurrent / _shieldMax;
        }
    }
    [ShowInInspector, ReadOnly] float _shieldCurrent;
    [ShowInInspector, ReadOnly] float _shieldMax;
    bool IsAtFullShield() => Mathf.Approximately(ShieldCurrent, _shieldMax);
    const int CONST_ShieldRegenAmount = 100;
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
                FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.floatingContainer);
                ft.SpawnMe("Immune", Color.deepPink);
                return true;
            }
        }
        return false;
    }


    public void HealthInjectData(PassData pd)
    {
        if (pd.hasManaShield)
        {
            _shieldMax = pd.manaShieldPoints;
            ShieldCurrent = _shieldMax;
            return;
        }

        Br.combat.CombatEventRegistered(CombatEvent.BeginGetHit, pd.myBrain);
        FloatingText ft = Instantiate(Ga.me.floatingTextPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.floatingContainer);
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
            damageCalculation();
            Br.combat.CombatEventRegistered(CombatEvent.GetHit, pd.myBrain);
            void damageCalculation()
            {
                if (Br.status.HasEffect(Status.Effect.Invulnerable))
                {
                    ft.SpawnMe("Invulnerable!", Color.brown);
                    return;
                }

                float totalDamage = 0f;
                for (int i = 0; i < pd.damagePair.Length(); i++)
                {
                    float val = pd.damagePair.GetValue(i);
                    switch (val)
                    {
                        case < 0: //heal
                            psHeal.Play();
                            break;
                        case > 0:
                        {
                            ParticleSystem ps = _dictPsElements[pd.damagePair.GetKey(i)];
                            if (ps != null) ps.Play();
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
                ft.SpawnMe(pd.damagePair);
                return;

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


    void Update()
    {
        _timerBars += Time.deltaTime;
        if (_timerBars < 1f) return;
        
        _timerBars = 0f;
        if (!IsAtFullHealth())
        {
            HealthCurrent += Br.character.GetStat(Stats.RegenerationRate) * 0.01f;
            return;
        }
        if (!IsAtFullShield()) ShieldCurrent += CONST_ShieldRegenAmount * Time.deltaTime;
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



    public void Death(Brain brainThatKilledMe)
    {
        if (brainThatKilledMe != null) brainThatKilledMe.combat.CombatEventRegistered(CombatEvent.Kill, Br);
        Quaternion rot  = Quaternion.LookRotation(Br.myTransform.forward) * Quaternion.Euler(new Vector3(-90f, 0f, 0f));
        ParticleSystem ps = Instantiate(Ga.me.psDeath, Br.myTransform.position, rot,Ga.me.transform);
        ps.Play();
        Ga.me.team.Death(Br);
       // Destroy(_healthBar.gameObject);
        Destroy(_pointer.gameObject);
        Destroy(Br.gameObject);
    }

}