using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;

public class FlashEffect : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField] SkinnedMeshRenderer[] _meshRenderers;
    [SerializeField] Material _flashMaterial;
    [SerializeField, Self] Health _health;
    
    [Header("Settings")]
    [SerializeField] int _flashCount = 1;
    [SerializeField] float _flashInterval = 0.1f;
    
    Material _originalMaterial;

    void Awake()
    {
        _originalMaterial = _meshRenderers[0].material;
    }

    void Start()
    {
        _health.CurrentHealth.AddListener(health => StartCoroutine(Flash()));
    }

    IEnumerator Flash()
    {
        for (int i = 0; i < _flashCount; i++)
        {
            SetMaterial(_flashMaterial);
            yield return new WaitForSeconds(_flashInterval);
            SetMaterial(_originalMaterial);
        }
    }

    void SetMaterial(Material mat) => Array.ForEach(_meshRenderers, r => r.material = mat);
}
