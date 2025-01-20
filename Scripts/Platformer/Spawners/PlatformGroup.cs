using System.Collections.Generic;
using UnityEngine;

public class PlatformGroup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _lastPlatform;
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Door _doorPrefab;
    [SerializeField] List<Transform> _enemySpawnPoints;
    [SerializeField] List<Transform> _doorSpawnPoints;
    
    [Header("Settings")]
    [SerializeField] Vector2 _enemyCountRange;
    [SerializeField] int _doorSpawnPercentage = 20;
    
    public Vector3 PlatformGroupEndPos => _lastPlatform.position;

    void Awake()
    {
        if (gameObject.name == "Section1_First")
        {
            // Don't spawn anything on the first platform group - The enemies should be there by default
            return;
        }
        
        int enemyCount = Random.Range((int)_enemyCountRange.x, (int)_enemyCountRange.y);
        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPoint = GetRandomEnemySpawnPoint();
            if (spawnPoint == null)
                break;
            
            Instantiate(_enemyPrefab, spawnPoint.position, _enemyPrefab.transform.rotation, transform);
        }
        
        if (Random.Range(0, 100) < _doorSpawnPercentage)
        {
            Instantiate(_doorPrefab, GetRandomDoorSpawnPoint().position, _doorPrefab.transform.rotation);
        }
    }

    Transform GetRandomEnemySpawnPoint()
    {
        while (_enemySpawnPoints.Count > 0)
        {
            Transform enemy = _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Count)];
            _enemySpawnPoints.Remove(enemy);
            if (enemy == null)
                continue;
            
            return enemy;
        }
        
        return null;
    }

    Transform GetRandomDoorSpawnPoint()
    {
        while (_enemySpawnPoints.Count > 0)
        {
            Transform door = _doorSpawnPoints[Random.Range(0, _doorSpawnPoints.Count)];
            _doorSpawnPoints.Remove(door);
            if (door == null)
                continue;
            
            return door;
        }
        
        return null;
    }
}