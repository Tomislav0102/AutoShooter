using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Ga : MonoBehaviour
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
    public SpecialUi specialUi;
    
    
    
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
        bool debug = false;
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
