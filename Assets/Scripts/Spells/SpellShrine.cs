using UnityEngine;

public class SpellShrine : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            if (!_spell.pd.hasStats) return;
            myTransform.Rotate(Vector3.up, Random.Range(0f, 360f));
            if (myPs is not null)
            {
                ParticleSystem.MainModule main = myPs.main;
                main.startColor = Ga.me.gameData.GetStatGroup(_spell.pd.stats[0].stat).col;
            }

            if (myLight is not null)
            {
                myLight.color =  Ga.me.gameData.GetStatGroup(_spell.pd.stats[0].stat).col;
            }
        }
    }
    SpellMain _spell;
    [SerializeField] Transform myTransform;
    [SerializeField] Light myLight;
    [SerializeField] ParticleSystem myPs;
    [SerializeField] Stats myStat;
}
