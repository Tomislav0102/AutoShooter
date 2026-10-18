using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Test : SerializedMonoBehaviour
{
    public Brain brain;
    public BuffStats buffStat;
    public BuffEffects[] buffEffects;

    public AnimationCurve curve;
    public Transform bomb, spawnPoint, targetPoint;
    public float speed = 1;
    public float length;
    [Range(0f, 1f)] public float currentPos;
    [Button]
    void AddS()
    {
      //  brain.character.CharacterInjectData(GenChange.Add, buffStat);
    }
    [Button]
    void Remove()
    {
    }
    

    void Update()
    {
        // float fl = Mathf.PingPong(Time.time * speed, length);
        // bomb.position = new Vector3(0f, curve.Evaluate(fl / length), fl);
        bomb.position = Vector3.Lerp(spawnPoint.position, targetPoint.position, currentPos);
    }
    
    void SetBuff(GenChange genChange)
    {
        for (int i = 0; i < buffEffects.Length; i++)
        {
            brain.status.StatusInjectData(genChange, buffEffects[i]);
        }
    }

}

public class RememberQueueStack
{
    Queue<int> _q;
    Stack<int> _s;


    public RememberQueueStack(int startSize)
    {
        _q = new Queue<int>();
        _s = new Stack<int>();
        for (int i = 0; i < startSize; i++)
        {
            _q.Enqueue(i);
            _s.Push(i);
        }
    }

    public void Add(int value)
    {
        _q.Enqueue(value);
        _s.Push(value);
    }
    public void Remove()
    {
        _q.Dequeue();
        _s.Pop();
       
    }
    public List<int> Result(bool showQueue)
    {
        List<int> result = new List<int>();
        if (showQueue)
        {
            foreach (int item in _q)
            {
                result.Add(item);
            }
            
        }
        else
        {
            foreach (int item in _s)
            {
                result.Add(item);
            }
            
        }
        return  result;
    }
    
}


public class AngledShot
{
    public Transform tr;
    public float maxHeight;
    public float distance;
    public float flightTime;
    public float posY, posZ, timer;

    void UpdateLoop()
    {
        timer += Time.deltaTime / flightTime;
        if (timer >= 1)
        {
            timer = 0f;
        }
        else if (timer > 0.5f)
        {
            posY = Mathf.Sqrt(1 - timer) * maxHeight * 2;
        }
        else
        {
            posY = Mathf.Sqrt(timer) * maxHeight * 2;
        }
        posZ += Time.deltaTime * distance;
        tr.position = new Vector3(0f, posY, posZ);
    }
}


