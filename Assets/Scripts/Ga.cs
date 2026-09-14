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
    [SerializeField] Transform parLevelUpCards;
    [HideInInspector] public LevelUpCard[] levelUpCards;
    [HideInInspector] public WaitForSeconds wait00;
    [HideInInspector] public WaitForSeconds wait01;
    [HideInInspector] public WaitForSeconds wait02;
    [HideInInspector] public WaitForSeconds wait20;
    [HideInInspector] public WaitForSeconds wait30;
    [Title("Debug")] 
    public bool debug;

    void Awake()
    {
        me = this;
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
        levelUpCards = Utils.AllChildren<LevelUpCard>(parLevelUpCards);
        team = new TeamManager();
        wait00 = Utils.GetWait(0f);
        wait01 = Utils.GetWait(0.1f);
        wait02 = Utils.GetWait(0.2f);
        wait20 = Utils.GetWait(2f);
        wait30 = Utils.GetWait(3f);
        // SceneManager.LoadScene(gameData.sceneLevel, LoadSceneMode.Additive);
        #if (!UNITY_EDITOR)
        SceneManager.LoadScene(gameData.sceneLevel, LoadSceneMode.Additive);
        #endif
    }

    void Start()
    {
       // Utils.ActivateOneArrayElement(players, PlayerPrefs.GetInt(gameData.prefsTestChosenPlayer));
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

    public void InjectSkills(SoSkill[] skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            levelUpCards[i].InjectSkill(skills[i]);
        }
    }
}