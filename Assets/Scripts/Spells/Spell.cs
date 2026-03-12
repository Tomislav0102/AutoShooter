using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SpellData
{
    public Element element;
    public float damage;
    public float areaOfEffect;
    public float speed;
    [Tooltip("0 - instant effect")]
    public float lifeTime;
    [Title("Targeting")]
    [ReadOnly] public  LayerMask layTarget;
    [ReadOnly] public Transform tarTransform;

}
public class Spell : EventBus
{
    public SpellData myData;
    [HideInInspector] public Transform myTransform;
    protected Rigidbody myRigid;
    protected Collider myCollider;
    protected System.Action<Transform> onHit;
    protected bool oneHitSwitch;
    float _timerLife;



    public virtual void InitializeMe(SpellData dat,System.Action<Transform> hitAction = null) 
    {
        myData = dat;
        oneHitSwitch = dat.lifeTime == 0;
        InitializeMe(hitAction);
    }
    public virtual void InitializeMe(LayerMask layersToTarget, System.Action<Transform> hitAction = null) 
    {
        myData.layTarget = layersToTarget;
        InitializeMe(hitAction);
    }
    protected virtual void InitializeMe(System.Action<Transform> hitAction = null) //if spellData is from inspector
    {
        onHit = hitAction;
        myTransform = transform;
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
        myRigid.isKinematic = true;
        myTransform.localScale = myData.areaOfEffect * Vector3.one;

        Vector3 dir = myTransform.forward;
        if (myData.tarTransform != null) dir = Utils.Direction(myTransform, myData.tarTransform);
        myTransform.forward = dir.normalized;
    }
    protected void Update()
    {
        if (myData.lifeTime > 0)
        {
            _timerLife += Time.deltaTime;
            if (_timerLife >= myData.lifeTime) StartCoroutine(Delay());
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForFixedUpdate();
        OnEnd();
    }

    protected virtual void OnEnd()
    {
        Destroy(gameObject);
    }
}
