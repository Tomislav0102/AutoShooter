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
                main.startColor = Ga.me.gameData.GetStatGroup(_spell.pd.stats[0].stat).col;
            }
            if (myMesh is not null)
            {
                myMesh.material = Ga.me.gameData.GetStatGroup(_spell.pd.stats[0].stat).mat;
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

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }
    

}
