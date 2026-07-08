using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ga : MonoBehaviour
{
    public Transform parPointers;
    public RectTransform offScreenPointerPrefab;
    public Drop dropPrefab;
    public static Ga me;
    [SerializeField] Transform parWaypoints;
    [HideInInspector] public Transform[] waypoints;

    [BoxGroup("Particles")] 
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
    public Transform cameraRigTransform;
    [HideInInspector] public Camera cam;
    public TeamManager team =  new TeamManager();
    public Material matSeeThroughWalls;
    public TomoJoystick.Joystick joystick;
    public UltimateUi ultimateUi;
    [Title("Debug")] 
    public bool debug;
    
    
    void Awake()
    {
        me = this;
        cam = cameraRigTransform.GetComponentInChildren<Camera>();
        waypoints = Utils.AllChildren<Transform>(parWaypoints);
#if (!UNITY_EDITOR)
        SceneManager.LoadScene(gameData.SceneLevel(), LoadSceneMode.Additive);

#endif
    }

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }

    void LateUpdate()
    {
        OffScreens();
        void OffScreens()
        {
        }
    }
    

    void CallEv_OnCharDeath(Brain brainDead)
    {
        team.CallEv_OnCharDeath(brainDead);
    }

}