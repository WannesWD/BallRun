using Cinemachine;
using UnityEngine;

public class ValuableItemBehavior : MonoBehaviour
{
    private PlayerCharacter _playerCharacter = null;

    [SerializeField] private LayerMask _itemLayer = 0;
    [SerializeField] private GameObject _handPosition = null;
    [SerializeField] private float _throwPower = 200f;
    [SerializeField] private float _pickupRange = 2.0f;
    [SerializeField] private float _maxThrwonTime = 1.0f;
    [SerializeField] private Rigidbody _rigidBody = null;
    [SerializeField] private SphereCollider _sphereCollider = null;
    [SerializeField] private CinemachineVirtualCamera _camera = null;
    [SerializeField] private ValuableItemHealth _itemHealth = null;

    private float _currentThrownTime = 0.0f;
    private bool _attached = false;
    private Vector3 _respawnPos = Vector3.zero;

    public Vector3 RespawnPos
    {
        get { return _respawnPos; }
        set { _respawnPos = value; }
    }
    private void Start()
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

        if (player) _playerCharacter = player;

        _rigidBody.transform.localScale = Vector3.one * _pickupRange / 2;

        _respawnPos = transform.position;
    }

    void Update()
    {
        //makes sure item doesnt re attach right after being thrown
        if(_currentThrownTime < _maxThrwonTime)
        {
            _currentThrownTime += Time.deltaTime;

            if (_currentThrownTime >= _maxThrwonTime)
                _attached = false;

            return;
        }

        if(_attached)
            HandleAttachedBehavior();
    }

    public void HandlePickup()
    {
        if (!_attached)
        {
            _rigidBody.transform.localScale = Vector3.one * 0.6f;
            _sphereCollider.enabled = false;
            _rigidBody.isKinematic = true;

            transform.position = _handPosition.transform.position;

            _attached = true;

            _currentThrownTime = _maxThrwonTime;

            _playerCharacter.ItemLayerValue = _itemLayer.value;
            _playerCharacter.HasItem(true);
            _itemHealth.wasThrown = false;
        }
    }
    public void ThrowItem()
    {   
        _rigidBody.transform.localScale = Vector3.one * _pickupRange / 2;
        _itemHealth.wasThrown = true;
        _sphereCollider.enabled = true;
        _rigidBody.isKinematic = false;
    
        _rigidBody.velocity = _camera.transform.forward * _throwPower;
    
        _playerCharacter.HasThrown = false;
        _currentThrownTime = 0f;
    
        _playerCharacter.HasItem(false);
    }
    private void HandleAttachedBehavior()
    {
        if (_camera == null || _rigidBody == null || _playerCharacter == null || _handPosition == null) return;

        transform.position = _handPosition.transform.position;

        if(_playerCharacter.SpawnPointChanged) { _respawnPos = _playerCharacter.RespawnPos; }
    }
    public void Respawn()
    {
        transform.position = _respawnPos;
        _itemHealth.wasThrown = false;
    }
}
