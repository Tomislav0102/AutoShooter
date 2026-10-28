using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpellStatue : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            if (!_spell.pd.hasStats) return;

            if (ps is not null)
            {
                ParticleSystem.MainModule main = ps.main;
                main.startColor = GetColor(_spell.pd.stats[0].stat);
            }
            if (myMesh is not null)
            {
                myMesh.material = GetMat(_spell.pd.stats[0].stat);
            }

            Vector2 v2 = new Vector2(0, Random.Range(1, 5));
            v2 = Utils.RotateV2(v2, Random.Range(0, 360f));
            Spell.myTransform.position += Utils.MakeV3(v2);
            Spell.myTransform.LookAt(Utils.LevelV3(Ga.me.team.playersBrain.myTransform.position));
        }
    }
    SpellMain _spell;
    [SerializeField] ParticleSystem ps;
    [SerializeField] MeshRenderer myMesh;
    [SerializeField] Trio[] data;
    bool _hasLanded;
    public Stats[] extraStats;

    public void OnEnter(Collider other)
    {
        if (other.transform != Ga.me.team.playersBrain.myTransform) return;
        Ga.me.uiManager.StatsInject(GenChange.Add, _spell.pd.stats[0].stat);
        _hasLanded = true;
    }

    public void OnExit(Collider other)
    {
        if (other.transform != Ga.me.team.playersBrain.myTransform) return;
        Ga.me.uiManager.StatsInject(GenChange.Remove, _spell.pd.stats[0].stat);
        _hasLanded = false;
    }

    void OnDisable()
    {
        if (_hasLanded) Ga.me.uiManager.StatsInject(GenChange.Remove, _spell.pd.stats[0].stat);
    }

    
    Color GetColor(Stats stat)
    {
        foreach (Trio trio in data)
        {
            if (stat == trio.stat) return trio.col;
        }
        return default;
    }
    Material GetMat(Stats stat)
    {
        foreach (Trio trio in data)
        {
            if (stat == trio.stat) return trio.mat;
        }
        return null;
    }

    [System.Serializable]
    struct Trio
    {
        public Stats stat;
        public Color col;
        public Material mat;
        
    }
}
