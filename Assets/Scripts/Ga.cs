using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ga : MonoBehaviour
{
    public static Ga me;
    [SerializeField] GameObject[] players;
    public CameraRig camRig;
    public Transform parPointers;
    public RectTransform offScreenPointerPrefab;
    public Drop dropPrefab;
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;

    [BoxGroup("Particles prefabs")] 
    public ParticleSystem psGenericImpact, psSpawn, psDeath, psDecalFire;
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
    public TeamManager team;
    public TomoJoystick.Joystick joystick;
    public UltimateUi ultimateUi;
    [Title("Debug")] 
    public bool debug;
    
    void Awake()
    {
        me = this;
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
        team = new TeamManager();
        // SceneManager.LoadScene(gameData.sceneLevel, LoadSceneMode.Additive);
#if (!UNITY_EDITOR)
        SceneManager.LoadScene(gameData.sceneLevel, LoadSceneMode.Additive);

#endif
    }

    void Start()
    {
       // Utils.ActivateOneArrayElement(players, PlayerPrefs.GetInt(gameData.prefsTestChosenPlayer));
    }

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }
    

    void CallEv_OnCharDeath(Brain brainDead)
    {
        team.CallEv_OnCharDeath(brainDead);
    }

    public void BtnRestart()
    {
        SceneManager.LoadScene(gameData.sceneGame);
    }
    public void BtnSpawnEnemies()
    {
       LevelMan.SpawnEnemies();
    }
    public void BtnBackToMain()
    {
        SceneManager.LoadScene(gameData.sceneMain);
    }


}