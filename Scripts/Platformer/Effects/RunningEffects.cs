using System;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunningEffects : ValidatedMonoBehaviour
{   
    [SerializeField] AudioSource _footstepsAudio;
    [SerializeField] RobotController _robotController;
    [SerializeField] AudioClip[] _footstepAudioClips;
    
    [SerializeField] AudioSource _jumpAudio;

    void Start()
    {
        _robotController.Jumped += OnJump;
    }

    public void PlayFootStepIfGrounded()
    {
        if (_robotController.IsGrounded)
        {
            _footstepsAudio.clip = _footstepAudioClips[Random.Range(0, _footstepAudioClips.Length)];
            _footstepsAudio.Play();
        }
    }

    void OnJump()
    {
        _jumpAudio.Play();
    }
}
