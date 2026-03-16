using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestApp
{

    public class ChController : MonoBehaviour
    {
        public Rigidbody myRigid;
        public float speed = 10;
        public float bounceTime = 1;

        float _vel;
        float _minDistance = 1f;
        bool _isBouncing;


        void Update()
        {
            Vector3 dir = GameManager.Instance.plTransform.position - transform.position;
            if (!_isBouncing) transform.forward = dir.normalized;
            if (dir.magnitude > _minDistance) _vel = speed;
            else
            {
                _vel = 0f;
            }
        }

        void FixedUpdate()
        {
            myRigid.AddRelativeForce(_vel * Vector3.forward);
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
    }
}
