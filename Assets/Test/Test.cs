using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class Test : SerializedMonoBehaviour
{
    public ParticleSystem.ColorBySpeedModule mm;
    public Brain brain;
    public BuffStats buffStat;
    public BuffEffects[] buffEffects;

    public SoSkill soSkill;
    public SoSkill SsoSkill
    {
        get => _soSkill;
        set => _soSkill = value;
    }
    SoSkill _soSkill;
    public int segment;
    public int iterations;

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
        // for (int i = 0; i < iterations; i++)
        // {
        //     switch (segment)
        //     {
        //         case 0:
        //             if (brain.skills.TryGetFromGroup(SkillName.SeismicAnchorage, out SoSkill skill)) {}
        //             break;
        //         case 1:
        //             if (soSkill is null){}
        //             break;
        //         case 2:
        //             if (SsoSkill is null){}
        //             break;
        //     }
        // }
    }
    
    void SetBuff(GenChange genChange)
    {
        for (int i = 0; i < buffEffects.Length; i++)
        {
            brain.status.StatusInjectData(genChange, buffEffects[i]);
        }
    }

}

public class RemeberQueueStack
{
    Queue<int> _q;
    Stack<int> _s;


    public RemeberQueueStack(int startSize)
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


