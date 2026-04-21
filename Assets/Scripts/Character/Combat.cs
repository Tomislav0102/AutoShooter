using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : MonoBehaviour, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _searchWait = Random.Range(0f, 0.2f) + 0.5f;
            _targets = value.faction == Faction.GoodGuys ? Ga.me.team[Faction.BadGuys] : Ga.me.team[Faction.GoodGuys];
            StartCoroutine(SearchTargetCoroutine());
            _isPlayer = Ga.me.playerTransform == value.myTransform;
            if (_isPlayer)
            {
                _pLoco = Br.loco as P_Loco;
                _playerEngageDistance = _pLoco.engageDistance;
            }
            else
            {
                _eLoco = Br.loco as E_Loco;
            }
        }
    }
    Brain _br;
    public bool IsInitialized { get; set; } //only called in children (because they're on scene)
    [field: SerializeField] public virtual Transform MyTarget { get; set; }
    protected float distanceToTarget;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
    HashSet<Transform> _targets;
    
    //cache
    protected Dictionary<Element, float> damMelee = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damRanged = new Dictionary<Element, float>();
    protected Dictionary<Element, float> damUltimate = new Dictionary<Element, float>();
    protected int counterHits;
    
    bool _isPlayer;
    float _playerEngageDistance = 10f;
    P_Loco _pLoco;
    protected E_Loco _eLoco;
    
    
    
    protected virtual void Update()
    {
        DistanceToTarget();
        
    }
    void DistanceToTarget()
    {
        if (MyTarget == null)
        {
            if (_isPlayer) _pLoco.Disp = Disposition.Relaxed;
            else
            {
                _eLoco.AttInputEnemy(false);
                _eLoco.Att1InputEnemy(false);
            }
        }
        else
        {
            distanceToTarget = Utils.Distance(Br.myTransform.position, MyTarget.position);
            if (_isPlayer) _pLoco.Disp = distanceToTarget < _playerEngageDistance ? Disposition.Fighting : Disposition.Wary;
        }
    }



    public virtual void FromAnimEv_Attack(int num = 0)
    {
        counterHits++;
    }
    public virtual void FromAnimEv_Ultimate(int num = 0) { }

    IEnumerator SearchTargetCoroutine()
    {
        yield return new WaitForSeconds(_searchWait * 2);
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(_searchWait);
        }
    }

    void CheckTarget()
    { 
      //  if (MyTarget != null) return;
        MyTarget = Utils.ClosestTransform(Br.myTransform.position, _targets, detectRange);
    }

}
