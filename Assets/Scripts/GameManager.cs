using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TomoJoystick;
using UnityEngine.Serialization;


public class GameManager : EventBus
{
    public static GameManager Instance;
    public Transform ground;
    public Transform cameraRigTransform;
    [HideInInspector] public Camera cam;
    public EnemyManager enemyManager;
    public PlayerControl playerControl;
    [HideInInspector] public Transform playerTransform;
    public HashSet<Transform> playersTeam = new HashSet<Transform>();
    public TomoJoystick.Joystick joystick;
    public PlayerProjectile projectilePrefab;
    public LayerMask layEnemies, layPlayer;


    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        
        cam = cameraRigTransform.GetComponentInChildren<Camera>();
        playerTransform = playerControl.transform;
        playersTeam.Add(playerTransform);
    }

    void FixedUpdate()
    {
        cameraRigTransform.position = Vector3.Lerp(cameraRigTransform.position, playerTransform.position, 0.2f);
    }

    protected override void CallEv_OnAllyDeath(Transform tr)
    {
        base.CallEv_OnAllyDeath(tr);
        if (gm.playersTeam.Contains(tr)) gm.playersTeam.Remove(tr);
        if (tr == playerTransform)
        {
            EventBus.OnPLayerDeath?.Invoke();
            print("Player is dead");

        }
        else
        {
            Destroy(tr.gameObject);
            print("Summon is dead");
        }
    }
}
