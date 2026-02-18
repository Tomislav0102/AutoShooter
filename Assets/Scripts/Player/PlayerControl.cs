using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerControl : EventBus
{
    InputAction _inputAttack;
    public bool isShooting; //debug
    [Title("References")]
    [SerializeField] Rigidbody myRigid;
    [Title("Current stats (weapons and player)")]
    [SerializeField] float moveSpeed;
    [SerializeField] float damage;
    [SerializeField] float bulletSpeed;
    float _timerShoot;
    const float CONST_Rof = 0.5f;
    [Title("Weapons active")]
    [SerializeField] bool front;
    [SerializeField] bool diagonal, side, back, homing;
    [SerializeField] int parallel, followUp, ricochet, pierce, bounce;
    float _timerFollowUp;
    const float CONST_FollowUp = 0.1f;
    const float CONST_HorGapBetweenProjectiles = 0.3f;



    protected override void OnEnable()
    {
        base.OnEnable();
        _inputAttack = InputSystem.actions.FindAction("Player/Jump");
        _inputAttack.Enable();
    }

    void Update()
    {
        if (followUp > 0)
        {
            if (_timerFollowUp < CONST_FollowUp)
            {
                _timerFollowUp += Time.deltaTime;
            }
        }
        
        if (!isShooting) return;
      //  if (_inputAttack.WasPressedThisFrame()) Shoot();
        _timerShoot += Time.deltaTime;
        if (_timerShoot > CONST_Rof)
        {
            _timerShoot = 0f;
            Shoot();
            StartCoroutine(ShootFollowUp());
        }
    }
    void FixedUpdate()
    {
        float camAngle = GameManager.Instance.cameraRigTransform.eulerAngles.y;
        Vector2 val = Quaternion.Euler(0, 0, -camAngle) * gm.joystick.value;
        myRigid.linearVelocity = Utils.To3d(val * moveSpeed);
    }

    IEnumerator ShootFollowUp()
    {
        for (int i = 0; i < followUp; i++)
        {
            yield return new WaitForSeconds(CONST_FollowUp);
            Shoot();
        }
    }
    void Shoot()
    {
        if (front)
        {
            SpawnProjectile(0f);
        }
        if (back)
        {
            SpawnProjectile(180f);
        }
        if (diagonal)
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnProjectile(45 * (i * 2 - 1));
            }
        }
        if (side)
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnProjectile(90 * (i * 2 - 1));
            }
        }
        if (homing)
        {
            Transform closest = Utils.ClosestTransform(transform.position, gm.enemyManager.allEnemies);
            if (closest != null)
            {
                Vector3 dir = (closest.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                SpawnProjectile(angle);
            }
        }

        _timerFollowUp = 0f;

        void SpawnProjectile(float rotation)
        {
            for (int i = 0; i < parallel + 1; i++)
            {
                float xOffset = i * CONST_HorGapBetweenProjectiles;
                PlayerProjectile projectile = Instantiate<PlayerProjectile>(gm.projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
                projectile.transform.Rotate(rotation * Vector3.up);
                projectile.transform.Translate(xOffset * Vector3.right, Space.Self);
                 float width = (parallel + 1) * CONST_HorGapBetweenProjectiles;
                 projectile.transform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);
                
                ProjectilePassData passData = new ProjectilePassData((string message) =>
                {
                    print(message);
                }, damage, bulletSpeed, ricochet, pierce, bounce);
                projectile.InitializeMe(passData);
            }

        }
    }

}
