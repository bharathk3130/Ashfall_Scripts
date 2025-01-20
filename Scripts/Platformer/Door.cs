using DG.Tweening;
using UnityEngine;


public class Door : MonoBehaviour
{
    [SerializeField] GameObject _textGameObject;
    [SerializeField] float _spriteMaskRange;
    
    [SerializeField] float _textAnimationDuration = 0.3f;
    
    Transform _spriteMask;
    
    bool _playerAtDoor;
    bool _maskIsHovering;
    
    bool IsDoorEnabled => _playerAtDoor && _maskIsHovering;
    
    public void Init(Transform spriteMask) => _spriteMask = spriteMask;

    void Start()
    {
        _spriteMask = Singleton.Instance.SpriteMaskTransform;
    }

    void Update()
    {
        _maskIsHovering = Vector3.Distance(transform.position, _spriteMask.position) < _spriteMaskRange;
        if (_maskIsHovering)
        {
            if (!_textGameObject.gameObject.activeSelf)
            {
                CancelInvoke(nameof(DisableText));
                _textGameObject.SetActive(true);
                _textGameObject.transform.DOScale(Vector3.one, _textAnimationDuration).SetEase(Ease.OutBounce);
            }
        } else
        {
            if (_textGameObject.gameObject.activeSelf)
            {
                _textGameObject.transform.DOScale(Vector3.zero, _textAnimationDuration).SetEase(Ease.InBounce);
                Invoke(nameof(DisableText), _textAnimationDuration);
            }
        }

        if (IsDoorEnabled)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Victory!");
                Invoke(nameof(ReloadScene), 3);
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _playerAtDoor = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _playerAtDoor = false;
        }
    }

    void ReloadScene() => SceneLoader.ReloadScene();
    void DisableText() => _textGameObject.SetActive(false);
}
