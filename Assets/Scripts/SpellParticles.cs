using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class SpellParticles
{
    [FormerlySerializedAs("ps")] [SerializeField] ParticleSystem[] psStart = System.Array.Empty<ParticleSystem>();
    [SerializeField] ParticleSystem[] psOnHit = System.Array.Empty<ParticleSystem>();
    public enum ParticleSizeChange
    {
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code because it will have its default scale defined in inspector (e.g. fireball)
        Emission_Shape, 
        Velocity_Over_Lifetime, //not used
        Other_None
    }
    [FormerlySerializedAs("particleSizeChange")] public ParticleSizeChange particleSizeChangeStart;

    public void InitializeMe(SpellMain spellMain)
    {
        spellMain.onHitTarget += (Brain br) =>
        {
           // if (psOnHit.Length == 0) return;
            foreach (ParticleSystem particleSystem in psOnHit)
            {
                particleSystem.Play();
            }
        };
         if (psStart.Length == 0) return;
        // foreach (ParticleSystem particleSystem in psStart)
        // {
        //     if (particleSystem == null)
        //     {
        //        return;
        //     }
        // }

        switch (particleSizeChangeStart)
        {
            case ParticleSizeChange.Emission_Shape:
                foreach (ParticleSystem particleSystem in psStart)
                {
                    var emission = particleSystem.emission;
                    emission.rateOverTime = spellMain.spell.areaOfEffect * 5;
                    var shape = particleSystem.shape;
                    shape.radius = spellMain.spell.areaOfEffect * 0.5f;
                }
                break;
            case ParticleSizeChange.TransformScale:
                psStart[0].transform.parent.localScale = spellMain.spell.areaOfEffect * Vector3.one;
                break;
            case ParticleSizeChange.Velocity_Over_Lifetime:
                foreach (ParticleSystem particleSystem in psStart)
                {
                    ParticleSystem.MainModule myMain = particleSystem.main;
                    myMain.duration = spellMain.spell.lifeTime;
                    myMain.startLifetime = spellMain.spell.lifeTime;
                    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
                    velocityOverLifetime.y = (spellMain.transporter as BulletTransporter).speed;
                }
                break;
        }
        foreach (ParticleSystem particleSystem in psStart)
        {
            particleSystem.Play();
        }
    }

    public void Stop()
    {
        foreach (ParticleSystem particleSystem in psStart)
        {
            particleSystem.Stop();
        }
    }

}