using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Combat : SerializedMonoBehaviour, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _searchWait = Random.Range(0f, 0.2f) + 0.5f;
            _targets = value.faction == Faction.Player ? Ga.me.team[Faction.Monsters] : Ga.me.team[Faction.Player];
            StartCoroutine(SearchTargetCoroutine());
            _isPlayer = Ga.me.playerTransform == value.myTransform;
            if (_isPlayer)
            {
                _pLoco = Br.loco as P_Loco;
                _playerEngageDistance = _pLoco.engageDistance;
            }
        }
    }
    Brain _br;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    [field: SerializeField] public virtual Transform MyTarget { get; set; }
    protected float distance;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
    HashSet<Transform> _targets;
    
    bool _isPlayer;
    float _playerEngageDistance = 10f;
    P_Loco _pLoco;
    
    protected virtual void Update()
    {
        DistanceToTarget();
        
        void DistanceToTarget()
        {
            if (MyTarget == null)
            {
                distance = -1;
                if (_isPlayer) _pLoco.Disp = Disposition.Relaxed;
            }
            else
            {
                distance = Utils.Distance(Br.myTransform.position, MyTarget.position);
                if (_isPlayer) _pLoco.Disp = distance < _playerEngageDistance ? Disposition.Fighting : Disposition.Wary;
            }
        }
    }



    public virtual void FromAnimEv_Attack(int num = 0) { }
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
