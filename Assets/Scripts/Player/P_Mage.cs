using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class WalkTrail
{
    Brain _br;
    float _timer;
    float _pauseBetweenSpawns = 0.1f;
    HashSet<Collider> _spawns;
    GameObject _spellPrefab;
    float _radius;

    /// <summary>
    /// Constructor should be used for resetting too (because Hashset keeps on growing with every spawn)
    /// </summary>
    /// <param name="br"></param>
    /// <param name="spellPrefab"></param>
    public WalkTrail(Brain br, GameObject spellPrefab)
    {
        _br = br;
        _spellPrefab = spellPrefab;
        _radius = spellPrefab.GetComponent<Spell>().radius;
        _spawns = new HashSet<Collider>();
    }
    

    public void UpdateLoop()
    {
        _timer += Time.deltaTime;
        if (_timer >= _pauseBetweenSpawns)
        {
            _timer = 0;
            Collider[] colliders = Physics.OverlapSphere(_br.myTransform.position, _radius * 2);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (_spawns.Contains(colliders[i])) return;
            }

            Spell spell = GameObject.Instantiate(_spellPrefab, _br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform).GetComponent<Spell>();
            spell.InitializeMe(_br);
            _spawns.Add(spell.myCollider);
        }

    }
} 

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
    WalkTrail _walkTrail;
    
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            value.loco.lookAtTarget = false;
            _walkTrail = new WalkTrail(value, Ga.me.spells.fireWalk);
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        float angle = 180f / (numOfHomingMissiles + 1);
        for (int i = 0; i < numOfHomingMissiles; i++)
        {
            S_Homing spell = Instantiate(Ga.me.spells.homing, Ga.me.spells.myTransform).GetComponent<S_Homing>();
            spell.homingTarget = Br.combat.MyTarget;
            spell.myTransform.position = Br.myTransform.position;
            spell.myTransform.forward = -Br.myTransform.right;
            spell.myTransform.rotation *= Quaternion.Euler(0f, angle * (i + 1), 0f);;
            spell.myMesh.position = new Vector3(spell.myMesh.position.x, spawnPoint.position.y, spell.myMesh.position.z);
            spell.InitializeMe(Br);
        }
    }

    protected override void Update()
    {
        base.Update();
        _walkTrail.UpdateLoop();
    }
}
