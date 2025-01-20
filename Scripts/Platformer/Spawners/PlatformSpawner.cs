using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlatformGroup[] _platformGroupPrefabs;
    [SerializeField] PlatformGroup[] _initialPlatformGroups;
    [SerializeField] Transform _environmentParent;
    [SerializeField] Transform _camTransform;

    [Header("Settings")]
    [SerializeField] Vector2 _blocksGapRange;

    List<PlatformGroup> _spawnedPlatformGroups = new();
    float _blockWidth = 2.12f;

    void Awake()
    {
        foreach (PlatformGroup platformGroup in _initialPlatformGroups)
        {
            _spawnedPlatformGroups.Add(platformGroup);
        }
    }

    void Update()
    {
        if (_camTransform.position.x > SecondLastPlatformPosX())
        {
            SpawnPlatform(GetRandomPlatform());
        }
    }

    void SpawnPlatform(PlatformGroup platformGroup)
    {
        Vector3 spawnPos = _spawnedPlatformGroups[^1].PlatformGroupEndPos;
        float gap = Random.Range(_blocksGapRange.x, _blocksGapRange.y) * _blockWidth;
        spawnPos.x += gap;
        PlatformGroup instance = Instantiate(platformGroup, spawnPos, Quaternion.identity, _environmentParent);
        _spawnedPlatformGroups.Add(instance);
    }
    
    PlatformGroup GetRandomPlatform() => _platformGroupPrefabs[Random.Range(0, _platformGroupPrefabs.Length)];
    float SecondLastPlatformPosX() => _spawnedPlatformGroups[^2].transform.position.x;
}
