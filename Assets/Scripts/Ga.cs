using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Ga : SerializedMonoBehaviour
{
    public static Ga me;
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;

    [BoxGroup("Particles")] 
    public ParticleSystem psSpawn, psDeath;
    public LevelManager LevelMan
    {
        get => _levelMan;
        set
        {
            _levelMan = value;
            EventBus.OnLevelLoaded?.Invoke();
        }
    }
    LevelManager _levelMan;
    public SoGameData gameData;
    public SoCharacter defCharacter;
    public SpellManager spells;
    public Transform barContainer;
    public RectTransform healthBarPrefab;
    public Transform floatingContainer;
    public FloatingText floatingTextPrefab;
    public Transform cameraRigTransform;
    [HideInInspector] public Camera cam;
    [HideInInspector] public Transform playerTransform;
    public Dictionary<Faction, HashSet<Transform>> team = new Dictionary<Faction, HashSet<Transform>>();
    public TomoJoystick.Joystick joystick;
    [FormerlySerializedAs("specialUi")] public UltimateUi ultimateUi;
    [Title("Debug")] 
    public bool debug;
    
    void Awake()
    {
        me = this;
        cam = cameraRigTransform.GetComponentInChildren<Camera>();
        team = new Dictionary<Faction, HashSet<Transform>>();
        for (int i = 0; i < System.Enum.GetNames(typeof(Faction)).Length; i++)
        {
            team.Add((Faction)i, new HashSet<Transform>());
        }
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
      // SceneManager.LoadScene(gameData.SceneLevel(), LoadSceneMode.Additive);
    }

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }



    void CallEv_OnCharDeath(Transform tr)
    {
        if (team[Faction.GoodGuys].Contains(tr))
        {
            team[Faction.GoodGuys].Remove(tr);
            if (tr == playerTransform)
            {
                EventBus.OnPlayerDeath?.Invoke();
                if (debug) print("Player is dead");
            }
            else
            {
                Destroy(tr.gameObject);
                if (debug) print("Summon is dead");
            }
        }
        else if (team[Faction.BadGuys].Contains(tr))
        {
            team[Faction.BadGuys].Remove(tr);
            Destroy(tr.gameObject);
            if (debug) print("enemy is dead");
        }

    }

}

[System.Serializable]
public class SpellParticles
{
    public enum ParticleSizeChange
    {
        Emission_Shape, 
        TransformScale, //ps needs to have empty parent that will be scaled. Ps.transform is never scaled by code because it will have its default scale defined in inspector (e.g. fireball)
        Other
    }
    [SerializeField] ParticleSystem ps;
    public ParticleSizeChange particleSizeChange;

    public void InitializeMe(float areaOfEffect)
    {
        if (ps == null) return;
        switch (particleSizeChange)
        {
            case  ParticleSizeChange.Emission_Shape:
                var emission = ps.emission;
                emission.rateOverTime = areaOfEffect * 5;
                var shape = ps.shape;
                shape.radius = areaOfEffect * 0.5f;
                break;
            case  ParticleSizeChange.TransformScale:
                ps.transform.parent.localScale = areaOfEffect * Vector3.one;
                break;
        }
        ps.Play();
    }
}