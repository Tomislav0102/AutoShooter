using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellVisual : MonoBehaviour
{
    SpellMain _main;
    [SerializeField] ParticleSystem psDefault;
    Light _lightDefault;
    [SerializeField] ParticleSystem psHit;
    Light _lightHit;
    enum ParticleSizeChange
    {
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code because it will have its default scale defined in inspector (e.g. fireball)
        Emission_Shape, 
        Velocity_Over_Lifetime, //not used
        Other_None
    }
    [SerializeField] ParticleSizeChange particleSizeChangeStart;
    [SerializeField] Transform myMesh;

    
    public void InitializeMe(SpellMain main)
    {
        _main = main;
        if (psDefault != null) _lightDefault = psDefault.GetComponent<Light>();
        if (_lightDefault != null) _lightDefault.enabled = false;
        if (psHit != null) _lightHit = psHit.GetComponent<Light>();
        if (_lightHit != null) _lightHit.enabled = false;
        _main.onHitTarget += (Brain br) =>
        {
            if (psHit != null) psHit.Play();
            StartCoroutine(LightDelay(_lightDefault));
        };

        if (psDefault != null)
        {
            switch (particleSizeChangeStart)
            {
                case ParticleSizeChange.Emission_Shape:
                    var emission = psDefault.emission;
                    emission.rateOverTime = _main.spell.areaOfEffect * 5;
                    var shape = psDefault.shape;
                    shape.radius = _main.spell.areaOfEffect * 0.5f;
                    break;
                case ParticleSizeChange.TransformScale:
                    psDefault.transform.parent.localScale = _main.spell.areaOfEffect * Vector3.one;
                    break;
                case ParticleSizeChange.Velocity_Over_Lifetime:
                    ParticleSystem.MainModule myMain = psDefault.main;
                    myMain.duration = _main.spell.lifeTime;
                    myMain.startLifetime = _main.spell.lifeTime;
                    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = psDefault.velocityOverLifetime;
                    velocityOverLifetime.y = (_main.transporter as BulletTransporter).speed;
                    break;
                case ParticleSizeChange.Other_None:
                    return;
            }
        }

    }

    public void PlayDefault()
    {
        if (psDefault != null) psDefault.Play();
        StartCoroutine(LightDelay(_lightDefault));
    }

    public void SetSpawnHeight(float height)
    {
       if (myMesh != null) myMesh.localPosition = new Vector3(0, height, 0);
       else transform.localPosition = new Vector3(0, height, 0);
    }

    IEnumerator LightDelay(Light myLight)
    {
        if (myLight == null) yield break;
        
        myLight.enabled = true;
        float time = 0.4f;
        float intensity = myLight.intensity;
        while (myLight.intensity > 0f)
        {
            myLight.intensity -= Time.deltaTime * intensity /  time;
            yield return null;
        }
        myLight.enabled = false;
    }
}

