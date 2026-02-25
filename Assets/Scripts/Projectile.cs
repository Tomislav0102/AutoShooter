using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    protected ProjectilePassData myData;

    public void InitializeMe(ProjectilePassData passData)
    {
        myData = passData;
        SetSpeed();
        Destroy(gameObject, 10);
    }

    protected void SetSpeed() => rigid.linearVelocity = myData.moveSpeed * transform.forward;

}
