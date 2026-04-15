using System;
using UnityEngine;

/// <summary>
/// Used to fine-tune best normTime for anim event, especially useful for long weapons
/// </summary>
public class AnimPreview : MonoBehaviour
{
    public Animator anim;
    public GenOrder clipToPlay;
    GenOrder _previousClipToPlay;

    string AnimName()
    {
        return "S" + ((int)clipToPlay).ToString();
    }
    [Range(0f, 1f)] public float track;
    float _previousTrack;


    void Start()
    {
        anim.speed = 0;
    }

    void OnAnimatorMove()
    {
        if (!Mathf.Approximately(_previousTrack, track) || _previousClipToPlay != clipToPlay)
        {
            anim.Play(AnimName(), 1, track);
            _previousTrack = track;
            _previousClipToPlay = clipToPlay;
        }
    }
}
