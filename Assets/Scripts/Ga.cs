using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ga : MonoBehaviour
{
    public static Ga me;
    
    public static System.Action OnLevelLoaded;
    public static System.Action<Brain, GenChange> OnBrainAddRemove;
    public static System.Action OnUltimateActivated;

    [SerializeField] GameObject[] players;
    public CameraRig camRig;
    public Drop dropPrefab;
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;
    public Material matFrozen;

    [FoldoutGroup("Particles prefabs")] 
    public ParticleSystem psGenericImpact, psSpawn, psDeath, psDecalFire, psArmorBreak;
    public LevelManager LevelMan
    {
        get => _levelMan;
        set
        {
            _levelMan = value;
            OnLevelLoaded?.Invoke();
        }
    }
    LevelManager _levelMan;
    public SoGameData gameData;
    public RunData runData;
    public SoCharacter defCharacter;
    public SpellManager spells;
    public TeamManager team;
    public UiManager uiManager;

    [HideInInspector] public WaitForSeconds wait00;
    [HideInInspector] public WaitForSeconds wait01;
    [HideInInspector] public WaitForSeconds wait02;
    [HideInInspector] public WaitForSeconds wait15;
    [HideInInspector] public WaitForSeconds wait20;
    [HideInInspector] public WaitForSeconds wait30;
    [HideInInspector] public WaitForSeconds wait100;
    [Title("Debug")] 
    public bool debug;
    
    void Awake()
    {
        me = this;
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
        team = new TeamManager();
        runData = new RunData();
        wait00 = Utils.GetWait(0f);
        wait01 = Utils.GetWait(0.1f);
        wait02 = Utils.GetWait(0.2f);
        wait15 = Utils.GetWait(1.5f);
        wait20 = Utils.GetWait(2f);
        wait30 = Utils.GetWait(3f);
        wait100 = Utils.GetWait(10f);
        #if (UNITY_EDITOR)
        #else
        SceneManager.LoadScene(gameData.sceneLevel, LoadSceneMode.Additive);
       // Utils.ActivateOneArrayElement(players, PlayerPrefs.GetInt(gameData.prefsTestChosenPlayer));
        #endif
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


