using Cinemachine;
using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineCameraFollow : ValidatedMonoBehaviour
{
    [SerializeField, Self] CinemachineVirtualCamera _virtualCam;
    [SerializeField] Transform _playerModel;
    
    [SerializeField] float _aheadOffset = 0.4f;
    [SerializeField] float _smoothSpeed = 5f;

    CinemachineTransposer _transposer;

    void Start()
    {
        _transposer = _virtualCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        bool facingRight = Mathf.Abs(_playerModel.localEulerAngles.y) < 30;
        Vector3 targetOffset = _transposer.m_FollowOffset;
        targetOffset.z = facingRight ? -_aheadOffset : _aheadOffset;

        // Smoothly interpolate m_FollowOffset to the target position
        _transposer.m_FollowOffset = Vector3.Lerp(_transposer.m_FollowOffset, targetOffset, _smoothSpeed * Time.deltaTime);
    }
}