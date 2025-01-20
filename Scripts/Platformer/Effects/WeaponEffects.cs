using KBCore.Refs;
using UnityEngine;

public class WeaponEffects : ValidatedMonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] AudioSource _shootAudio;
    [SerializeField] Gun _gun;

    [Header("Settings")]
    [SerializeField] float _pitchVariation = 0.1f;
    [SerializeField] float _volumeVariation = 0.1f;
    
    float _initialPitch;
    float _initialVolume;

    void Start()
    {
        _initialPitch = _shootAudio.pitch;
        _initialVolume = _shootAudio.volume;
        
        _gun.OnShoot += OnShoot;
    }

    void OnShoot()
    {
        _shootAudio.pitch = _initialPitch + Random.Range(-_pitchVariation, _pitchVariation);
        _shootAudio.volume = _initialVolume + Random.Range(-_volumeVariation, _volumeVariation);
        _shootAudio.Play();
    }
}
