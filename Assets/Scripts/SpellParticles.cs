using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpellParticles
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] ParticleSystem[] extraPs = Array.Empty<ParticleSystem>();
    public enum ParticleSizeChange
    {
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code because it will have its default scale defined in inspector (e.g. fireball)
        Emission_Shape, 
        Velocity_Over_Lifetime, //only for Sweeping Arc spell
        Other
    }
    public ParticleSizeChange particleSizeChange;

    public void InitializeMe(SpellControl spellControl)
    {
        if (ps == null) return;
        switch (particleSizeChange)
        {
            case  ParticleSizeChange.Emission_Shape:
                var emission = ps.emission;
                emission.rateOverTime = spellControl.spell.areaOfEffect * 5;
                var shape = ps.shape;
                shape.radius = spellControl.spell.areaOfEffect * 0.5f;
                break;
            case  ParticleSizeChange.TransformScale:
                ps.transform.parent.localScale = spellControl.spell.areaOfEffect * Vector3.one;
                break;
            case  ParticleSizeChange.Velocity_Over_Lifetime:
                List<ParticleSystem> particleSystems = new List<ParticleSystem>();
                particleSystems.Add(ps);
                particleSystems.AddRange(extraPs);
                foreach (var item in particleSystems)
                {
                    ParticleSystem.MainModule myMain = item.main;
                    myMain.duration = spellControl.spell.lifeTime;
                    myMain.startLifetime = spellControl.spell.lifeTime;
                    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = item.velocityOverLifetime;
                    velocityOverLifetime.y = (spellControl.transporter as BulletTransporter).speed;
                }
                break;
        }
        ps.Play();
    }

}