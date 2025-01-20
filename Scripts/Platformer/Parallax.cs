using System;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] BackgroundHelper[] _backgroundHelpers;
    [SerializeField] PlayerInput _playerInput;

    [SerializeField, Range(0, 1.5f)] float _parallaxMultiplier = 0.5f;

    void Start()
    {
        Array.ForEach(_backgroundHelpers, b => b.multiplier = 0);
    }

    void Update()
    {
        Array.ForEach(_backgroundHelpers, b => b.multiplier = _playerInput.Move.x * _parallaxMultiplier);
    }
}
