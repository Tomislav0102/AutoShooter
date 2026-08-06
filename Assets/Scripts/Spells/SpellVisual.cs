using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellVisual : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            if (psDefault != null)
            {
                _lightDefault = psDefault.GetComponent<Light>();
                if (_lightDefault != null) _lightDefault.enabled = false;
            }
            if (psHit != null)
            {
                _lightHit = psHit.GetComponent<Light>();
                if (_lightHit != null) _lightHit.enabled = false;
            }
            if (myTiledSpriteRenderer != null) myTiledSpriteRenderer.size = new Vector2(myTiledSpriteRenderer.size.x, value.areaOfEffect);
            value.onHitTarget += (Brain br) =>
            {
                if (psHit != null) psHit.Play();
                StartCoroutine(LightDelay(_lightDefault));
            };

            switch (sizeModifier)
            {
                case SizeModifierType.Emission_Shape:
                    var emission = psDefault.emission;
                    emission.rateOverTime = value.areaOfEffect * 5;
                    var shape = psDefault.shape;
                    shape.radius = value.areaOfEffect * 0.5f;
                    break;
                case SizeModifierType.TransformScale:
                    transform.localScale = value.areaOfEffect * Vector3.one;
                    break;
                case SizeModifierType.Velocity_Over_Lifetime:
                    ParticleSystem.MainModule myMain = psDefault.main;
                    myMain.duration = value.lifeTime;
                    myMain.startLifetime = value.lifeTime;
                    ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = psDefault.velocityOverLifetime;
                    velocityOverLifetime.y = (value.transporter as BulletTransporter).speed;
                    break;
                case SizeModifierType.Other_None:
                    return;
            }
        }
    }
    SpellMain _spell;

    [SerializeField] ParticleSystem psDefault;
    Light _lightDefault;
    [SerializeField] ParticleSystem psHit;
    Light _lightHit;
    enum SizeModifierType
    {
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code because it will have its default scale defined in inspector (e.g. fireball)
        Emission_Shape, 
        Velocity_Over_Lifetime, //not used
        Other_None
    }
    [SerializeField] SizeModifierType sizeModifier;
    [SerializeField] Transform myMesh;
    [Tooltip("if != null, SizeModifierType should be Other_None")] [SerializeField] SpriteRenderer myTiledSpriteRenderer;

    


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

