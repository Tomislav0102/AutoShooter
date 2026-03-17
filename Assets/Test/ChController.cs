using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestApp
{

    public class ChController : MonoBehaviour
    {
        public Rigidbody myRigid;
        public Animator anim;
        public float speed = 10;
        public float bounceTime = 1;

        float _vel;
        float _minDistance = 1f;
        bool _isBouncing;
        int _move = Animator.StringToHash("move");

        public float myValue;
        void Awake()
        {
            GameManager.Instance.enemies.Add(transform);
        }

        void Update()
        {
            Vector3 dir = GameManager.Instance.plTransform.position - transform.position;
            dir.y = 0f;
            if (!_isBouncing) transform.forward = dir.normalized;
            if (dir.magnitude > _minDistance) _vel = speed;
            else
            {
                _vel = 0f;
            }

            if (transform.position.y < -10f) Destroy(gameObject);
        }

        void FixedUpdate()
        {
            myRigid.AddRelativeForce(_vel * Vector3.forward);
            anim.SetFloat(_move, myRigid.linearVelocity.sqrMagnitude);
            
            myValue = myRigid.linearVelocity.sqrMagnitude;
        }

        void OnCollisionEnter(Collision other)
        {
            if (!_isBouncing && other.collider.TryGetComponent(out IObstacle obstacle))
            {
                _isBouncing = true;
                float range = 90f;
                transform.forward = Quaternion.Euler(Random.Range(-range, range) * Vector3.up) * other.contacts[0].normal;
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (_isBouncing && other.collider.TryGetComponent(out IObstacle obstacle)) StartCoroutine(Delay());
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(bounceTime);
            _isBouncing = false;
        }

        void OnDestroy()
        {
            GameManager.Instance.enemies.Remove(transform);
        }
    }
}
