using UnityEngine;
using UnityEngine.Events;

public class SpellPeriod : MonoBehaviour, IIniSpell
{
    public SpellMain Spell { get; set; }

    [SerializeField] UnityEvent onHit;
    [SerializeField] float rateOfFire;
    float _timerPeriod = Mathf.Infinity;
    bool _oneHit;
    

    void Update()
    {
        if (!Spell.spellActive) return;
        if (Spell.MyPhase != SpellMain.Phase.SpellRuns) return;
        if (_oneHit) return;

        if (rateOfFire == 0)
        {
            onHit.Invoke();
            _oneHit = true;
            return;
        }
        
        _timerPeriod += Time.deltaTime;
        if (_timerPeriod > rateOfFire)
      //  if (_timer > Ga.me.gameData.rofSpells)
        {
            _timerPeriod = 0f;
            onHit.Invoke();
        }
    }


}
