using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerCharacter : BasicCharacter
{
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private InputActionReference _depthMovementAction;
    [SerializeField] private InputActionReference _widthMovementAction;

    [SerializeField] private UnityEvent _startRushingAir;
    [SerializeField] private UnityEvent _stopRushingAir;
    [SerializeField] private UnityEvent _startSlideSound;
    [SerializeField] private UnityEvent _stopSlideSound;

    private Vector3 _respawnPos = Vector3.zero;
    private float _respawnAngle = 0;
    
    private InputAction _sprintMovementAction;
    private InputAction _slideAndCrouchAction;
    private InputAction _jumpAction;
    private InputAction _throwAction;

    private float _mouseSens = 0.1f;

    private bool _prevSpeedLineState = false;
    private bool _itemThrown = false;
    private bool _spawnPointChanged = false;
    private bool _slideAudioPlaying = false;
    public Vector3 RespawnPos
    {
        get { return _respawnPos; }
        set { _respawnPos = value; }
    }

    public float RespawnAngle
    {
        get { return _respawnAngle; }
        set { _respawnAngle = value; }
    }

    public int ItemLayerValue
    {
        get {  return _movementBehavior.ItemLayerValue; }
        set { _movementBehavior.ItemLayerValue = value; }
    }
    public bool SpawnPointChanged
    {
        get { return _spawnPointChanged; }
        set { _spawnPointChanged = value; }
    }
    public bool HasThrown
    {
        get { return _itemThrown; }
        set { _itemThrown = value; }
    }
    public void HasItem(bool hasItem)
    {
        _movementBehavior.HasItem = hasItem;
    }
    protected override void Awake()
    {
        base.Awake();

        if (_inputAsset == null) return;

        _jumpAction = _inputAsset.FindActionMap("Gameplay").FindAction("Jump");
        _slideAndCrouchAction = _inputAsset.FindActionMap("Gameplay").FindAction("Slide/Crouch");
        _sprintMovementAction = _inputAsset.FindActionMap("Gameplay").FindAction("Sprint");
        _throwAction = _inputAsset.FindActionMap("Gameplay").FindAction("ThrowItem");

        _jumpAction.performed += HandleJumpInput;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState= CursorLockMode.Locked;
        _characterEffectManager.DrawSpeeLines(false);
        _respawnPos = transform.position;
    }

    private void OnEnable()
    {
        if(_inputAsset == null) return;

        _inputAsset.Enable();
    }
    private void OnDisable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Disable();  
    }

    private void Update()
    {
        HandleMovementInput();
        HandleAimingInnput();
        HandleDoubleJumpColorChange();
        HandleSpeedLineDrawing();
        HandleItemThrowing();
        HandleSlideInput(); 
        HandleSprintInput();
    }

    private void HandleSprintInput()
    {
        if (_sprintMovementAction == null) return;

        _movementBehavior.Sprint(_sprintMovementAction.IsPressed());
    }

    private void HandleMovementInput()  
    {
        if (_movementBehavior == null || _depthMovementAction == null || _widthMovementAction == null) return;

        float depthMovementInput = _depthMovementAction.action.ReadValue<float>();
        float widthMovementInput = _widthMovementAction.action.ReadValue<float>();

        Vector3 depthMovement = depthMovementInput * Vector3.back;  
        Vector3 widthMovement = widthMovementInput * Vector3.right;

        _movementBehavior.DesiredDepthMovementDirection = depthMovement;
        _movementBehavior.DesiredWidthMovementDirection = widthMovement;
    }

    private void HandleAimingInnput()
    {
        Vector2 relativeMousePos = Mouse.current.delta.ReadValue() * _mouseSens;
        _movementBehavior.ViewRotation = relativeMousePos;
    }

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        _movementBehavior.Jump();   
    }
    private void HandleSlideInput()
    {
        if (_slideAndCrouchAction == null) return;

        if (_slideAndCrouchAction.IsPressed())
            _movementBehavior.Crouch(true);
        else
            _movementBehavior.Crouch(false);

        if(!_slideAudioPlaying && _movementBehavior.IsOnGround && _movementBehavior.HasSpeedLines && _slideAndCrouchAction.IsPressed())
        {
            _startSlideSound?.Invoke();
            _slideAudioPlaying = true;
        }

        if (!_movementBehavior.IsOnGround || !_movementBehavior.HasSpeedLines || !_slideAndCrouchAction.IsInProgress())
        {
            _stopSlideSound?.Invoke();
            _slideAudioPlaying= false;
        }
    }
    private void HandleDoubleJumpColorChange()
    {
        bool hasJump = _movementBehavior.HasDoubleJump;

        _characterEffectManager.SetDoubleJumpColor(!hasJump);
    }
    private void HandleSpeedLineDrawing()
    {
        bool hasSpeedLines = _movementBehavior.HasSpeedLines;
        
        if(_prevSpeedLineState != hasSpeedLines)
        {
            if (hasSpeedLines) 
                _startRushingAir?.Invoke();
            else
                _stopRushingAir?.Invoke();

            _characterEffectManager.DrawSpeeLines(hasSpeedLines);
            _prevSpeedLineState = hasSpeedLines;
        }
    }

    private void HandleItemThrowing()
    {
        if (_throwAction == null) return;

        if(_throwAction.IsPressed() ) 
        {
            _pickupBehavior.ThrowItem();
        }
    }

    public ValuableItemBehavior getAttachedItem()
    {
        return _pickupBehavior.getAttachedItem();
    }

    public void Respawn()
    {
        transform.position = _respawnPos;
        _movementBehavior.Respawn(_respawnAngle);
    }

    public void HandleRushingSlideSound()
    {
        if(_movementBehavior.HasSpeedLines) { }
    }
}

