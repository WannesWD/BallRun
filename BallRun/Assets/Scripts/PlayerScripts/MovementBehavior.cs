using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class MovementBehavior : MonoBehaviour
{
    [SerializeField] private CapsuleCollider _capsuleCollider = null;
    [SerializeField] private GameObject _playerBody = null;
    [SerializeField] private CinemachineRecomposer _camera = null;
    [SerializeField] private GameObject _lookAtOrigin = null;
    [SerializeField] private float _movementSpeed = 1.0f;
    [SerializeField] private float _jumpStrength = 10.0f;
    [SerializeField] private float _maxMovementLockTime = 0.5f;
    [SerializeField] private int _maxNumJumps = 2;
    [SerializeField] private UnityEvent _onJumpEvent;

    private Rigidbody _rigidBody;

    private Vector2 _viewRotation = Vector2.zero;
    private Vector2 _totalRotation = Vector2.zero;

    private Vector3 _desiredDepthMovementDirection = Vector3.zero;
    private Vector3 _desiredWidthMovementDirection = Vector3.zero;

    private Vector3 _movementWhenWallHit = Vector3.zero;
    private Vector3 _wallRunMovementVector = Vector3.zero;
    private Vector3 _wallNormal = Vector3.zero;

    private GameObject _target;

    private int _itemLayerValue = -1;

    private bool _wasOnWall = false;
    private bool _sprinting = false;
    private bool _grounded = false;
    private bool _crouched = false;
    private bool _onWall = false;
    private bool _hasJump = true;
    private bool _hasItem = false;

    private int _currentJumpNum = 0;
    private int _currentCrouchBoostnum = 0;
    private int _maxNumCrouchBoosts = 1;

    private float _maxFOV = 0;
    private float _minFOV = 0;
    private float _currentFOV = 0;
    private float _lerpT = 0;
    private float _maxMovementVelocity = 0;
    private float _currentCameraDutchAngle = 0;
    private float _currentMovementLockTime = 0;
    private float _wallDistanceCheck = 0.8f;

    private const float FOV_LERP_SPEED = 0.1f;
    private const float VIEW_BASED_WALL_CHECK = 3f;
    private const float MOVEMENT_BASED_WALL_CHECK = 0.8f;
    private const float CAMERA_DUTCH_ANGLE = 5f;
    private const float GROUND_CHECK_DISTANCE = 0.5f;
    private const float MAX_CAM_ANGLE = 70f;

    private const string GROUND_LAYER = "Ground";
    private const string RUNWALL_LAYER = "RunWall";
    private const string CROUCHWALL_LAYER = "CrouchWall";

    public delegate void CharacterJumped(int maxJumpNum, int currentJumpNum);
    public event CharacterJumped OnJump;
    public Vector2 ViewRotation
    {
        get { return _viewRotation; }
        set { _viewRotation = value; }
    }

    public Vector3 DesiredWidthMovementDirection
    {
        get { return _desiredWidthMovementDirection; }
        set { _desiredWidthMovementDirection = value; }
    }
    public Vector3 DesiredDepthMovementDirection
    {
        get { return _desiredDepthMovementDirection; }
        set { _desiredDepthMovementDirection = value; }
    }

    public int ItemLayerValue
    {
        get { return _itemLayerValue; }
        set { _itemLayerValue = value; }
    }

    public bool HasItem
    {
        get { return _hasItem; }
        set { _hasItem = value; }
    }

    public bool HasSpeedLines
    {
        get { return _rigidBody.velocity.magnitude > _movementSpeed; }
    }

    public bool IsSprinting
    {
        get { return _sprinting; }
        set { _sprinting = value; }
    }
    public bool IsOnGround
    {
        get { return _grounded; }
        set { _grounded = value; }
    }
    public bool HasDoubleJump
    {
        get { return _hasJump; }
        set { _hasJump = value; }
    }
    public GameObject Target
    {
        get { return _target; }
        set { _target = value; }
    }
    public int CurrentJumpNum
    {
        get { return _currentJumpNum; }
    }
    public int MaxNumJumps
    {
        get { return _maxNumJumps; }
    }

    private void Awake()
    {
        _rigidBody= GetComponent<Rigidbody>();
        _totalRotation = new Vector2(0,0);
        _viewRotation = new Vector2(0, 0);
        _maxMovementVelocity = (_movementSpeed * 4);
        _minFOV = _camera.m_ZoomScale;
        _maxFOV = 1.25f;
    }

    private void FixedUpdate()
    {
        if (_onWall)
            HandleWallRunMovement();

        else HandleMovement();

        if (_grounded == false && _rigidBody.velocity.y <= 0)
        {
            //check for ground collisions
            _grounded = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down,
                GROUND_CHECK_DISTANCE, LayerMask.GetMask(GROUND_LAYER));
            if(_grounded) 
            { 
                _wasOnWall = false;
                _currentJumpNum = 0;
                OnJump?.Invoke(_maxNumJumps, _currentJumpNum);
            }
            if(_grounded && _crouched)
            {
                _rigidBody.velocity = _rigidBody.velocity.normalized * (_rigidBody.velocity.magnitude * 3);
            }
        }
        if (_grounded) { _hasJump = true; }

        if (_hasItem && _itemLayerValue == LayerMask.GetMask(CROUCHWALL_LAYER)) UnCrouch();
    }

    private void Update()
    {
        if(Time.timeScale == 0) return;
        HandleLookat();
        HandleWallCollision();
        HandleFOVChange();
    }
    
    private void HandleFOVChange()
    {
        float velocity = MathF.Min(_rigidBody.velocity.magnitude, (_maxMovementVelocity * 2));
        float velocityRatio = velocity / _maxMovementVelocity;

        _currentFOV = _minFOV + ((_maxFOV - _minFOV) * velocityRatio);

        _camera.m_ZoomScale = Mathf.Lerp(_camera.m_ZoomScale, _currentFOV, _lerpT);

        _lerpT = Time.deltaTime / FOV_LERP_SPEED;
    }

    //rotate character and camera based on mouse input
    private void HandleLookat()
    {
        if (_playerBody == null || _viewRotation == null || _lookAtOrigin == null) return;

        if (Mathf.Abs(_viewRotation.x) > 0 || Mathf.Abs(ViewRotation.y) > 0 )
        {
            Vector3 horizontalRotationAxis = Vector3.up;
            Vector3 verticalRotationAxis = Vector3.right;

            _lookAtOrigin.transform.Rotate(verticalRotationAxis, _totalRotation.y);
            _playerBody.transform.Rotate(horizontalRotationAxis, -_totalRotation.x);

            _totalRotation += _viewRotation;
            _totalRotation.y = Math.Clamp(_totalRotation.y, -MAX_CAM_ANGLE, MAX_CAM_ANGLE);

            _playerBody.transform.Rotate(horizontalRotationAxis, _totalRotation.x);
            _lookAtOrigin.transform.Rotate(verticalRotationAxis, -_totalRotation.y);
        }

    }
    private void HandleMovement() 
    {
        if (_rigidBody == null || _capsuleCollider == null) return;

        if(_currentMovementLockTime < _maxMovementLockTime)
        {
            _currentMovementLockTime += Time.deltaTime;
            if (_currentMovementLockTime > _maxMovementLockTime)
            {
                _wasOnWall = false;
                _hasJump = true;
                OnJump?.Invoke(_maxNumJumps, _currentJumpNum);
            }
            return;
        }

        //calculate and apply player movement
        float minCrouchVelocity = _movementSpeed * 0.5f;
        if(!_crouched || _rigidBody.velocity.magnitude < minCrouchVelocity)
        {
            Vector3 widthMovement = _desiredWidthMovementDirection.normalized;
            Vector3 depthMovement = _desiredDepthMovementDirection.normalized;
            Vector3 movement = depthMovement + widthMovement;
            movement.Normalize();
            float totalRotationRad = -_totalRotation.x / 180 * Mathf.PI;

            Vector2 rotate1 = new Vector2(Mathf.Cos(totalRotationRad), Mathf.Sin(totalRotationRad));
            Vector2 rotate2 = new Vector2(-Mathf.Sin(totalRotationRad), Mathf.Cos(totalRotationRad));

            if (_crouched)
                movement *= minCrouchVelocity;
            else if (_sprinting)
                movement *= _movementSpeed * 2.5f;
            else
                movement *= _movementSpeed;

            Vector3 trueMovement = new Vector3(rotate1.x * movement.x + rotate2.x * movement.z,
                                                0,
                                                rotate1.y * movement.x + rotate2.y * movement.z);

            Vector3 wallRunMovement = CalculateWallRunMovement(trueMovement, totalRotationRad);

            if (wallRunMovement != Vector3.zero)
                trueMovement = wallRunMovement;

            if (trueMovement.y == 0)
                trueMovement.y = _rigidBody.velocity.y;

            _rigidBody.velocity = trueMovement;
        }
    }

    public void Sprint(bool sprinting)
    {
        if (_sprinting == sprinting) return;
        

        if(!_crouched)
        {
            _sprinting = sprinting; 
        }
    }

    public void Jump()
    {
        if (_rigidBody == null) return;

        if (_currentJumpNum < (_maxNumJumps)) 
        {
            float jumpStrength = _jumpStrength;
            if(_crouched)
            {
                _crouched = false;
                UnCrouch();
                float minCrouchVelocity = _movementSpeed * 0.5f;
                if(_rigidBody.velocity.magnitude > minCrouchVelocity)
                {
                    _sprinting = true;
                }
                jumpStrength /= 2;
            }
        
            Vector3 prevVelocity = _rigidBody.velocity;
            prevVelocity.y = 0.1f;
            _rigidBody.velocity = prevVelocity;
        
            if(_onWall)
            {
                _wasOnWall = true;
                _onWall = false;
                _camera.m_Dutch = 0;
                _lookAtOrigin.transform.Rotate(Vector3.forward, -_currentCameraDutchAngle, Space.Self);
                _currentMovementLockTime = 0f;
            }
            Vector3 jumpVector = (_onWall || _wasOnWall) ? Vector3.up + _wallNormal : Vector3.up;
        
            
            _rigidBody.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);

            _grounded = false;
            _currentJumpNum++;
            
            OnJump?.Invoke(_maxNumJumps,_currentJumpNum);
            _onJumpEvent?.Invoke();
        }
    }
    public void Crouch(bool crouched)
    {
        if (_crouched == crouched) return;
        _crouched = crouched;

        if (_hasItem && _itemLayerValue == LayerMask.GetMask(CROUCHWALL_LAYER)) return;

        if(_crouched)
        {
            _rigidBody.transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);

            if((_grounded || _currentJumpNum == 1) && _currentCrouchBoostnum < _maxNumCrouchBoosts)
            {
                _rigidBody.velocity = _rigidBody.velocity.normalized * Mathf.Min((_rigidBody.velocity.magnitude * 2), _maxMovementVelocity);
            }
            _crouched = true;
        }

        if(!_crouched)
        {
            UnCrouch();
        }
    }

    //calculate player movement when wallrun starts
    private Vector3 CalculateWallRunMovement(Vector3 trueMovement, float totalRotationRad)
    { 
        RaycastHit viewHitInfo = new RaycastHit();
        RaycastHit movementHitInfo = new RaycastHit();
        Vector2 rotate1 = new Vector2(Mathf.Cos(totalRotationRad), Mathf.Sin(totalRotationRad));
        Vector2 rotate2 = new Vector2(-Mathf.Sin(totalRotationRad), Mathf.Cos(totalRotationRad));

        Vector3 viewVector = new Vector3(rotate1.x * Vector3.forward.x + rotate2.x * Vector3.forward.z,
                                            0,
                                            rotate1.y * Vector3.forward.x + rotate2.y * Vector3.forward.z);

        bool wallRunViewBased = Physics.Raycast(transform.position + Vector3.up, viewVector, out viewHitInfo, 3f, LayerMask.GetMask(RUNWALL_LAYER));
        bool wallRunMovementBased = Physics.Raycast(transform.position + Vector3.up, trueMovement.normalized, out movementHitInfo, 0.8f, LayerMask.GetMask(RUNWALL_LAYER));


        if (wallRunViewBased || wallRunMovementBased)
        {
            RaycastHit hitInfo = wallRunViewBased ? viewHitInfo : movementHitInfo;
            _wallDistanceCheck = wallRunViewBased ? VIEW_BASED_WALL_CHECK : MOVEMENT_BASED_WALL_CHECK;

            if (_hasItem && (_itemLayerValue == LayerMask.GetMask(RUNWALL_LAYER)))
            {
                return _rigidBody.velocity;
            }

            _movementWhenWallHit = wallRunViewBased ? viewVector : trueMovement;

            Vector3 wallNormal = hitInfo.normal;

            float cosAngleToNormal = Vector3.Dot(viewVector.normalized, wallNormal);                                            //angle between movemetnVector and wall normal
            float angleToWall = Mathf.Acos(cosAngleToNormal) - Mathf.PI / 2;                                                    //angle between movementVector and wall surface
            _currentCameraDutchAngle = CAMERA_DUTCH_ANGLE;

            if (Vector3.Cross(trueMovement.normalized, hitInfo.normal).y > 0)
            {
                angleToWall = -angleToWall;
                _currentCameraDutchAngle = -_currentCameraDutchAngle;
            }

            rotate1 = new Vector2(Mathf.Cos(totalRotationRad + angleToWall), Mathf.Sin(totalRotationRad + angleToWall));
            rotate2 = new Vector2(-Mathf.Sin(totalRotationRad + angleToWall), Mathf.Cos(totalRotationRad + angleToWall));

            Vector3 trueForward = new Vector3(rotate1.x * Vector3.forward.x + rotate2.x * Vector3.forward.z,
                                                0,
                                                rotate1.y * Vector3.forward.x + rotate2.y * Vector3.forward.z);

            trueMovement = (_movementSpeed * 3f) * trueForward.normalized;

            _wallRunMovementVector = trueMovement;

            trueMovement.y = _rigidBody.velocity.y / 2;

            _grounded = true;
            _currentJumpNum = 0;
            OnJump?.Invoke(_maxNumJumps, _currentJumpNum);
            _onWall = true;
            _wallNormal = hitInfo.normal;
            
            _camera.m_Dutch = _currentCameraDutchAngle;

            _lookAtOrigin.transform.Rotate(Vector3.forward, _currentCameraDutchAngle, Space.Self);

            return trueMovement;
        }
        return Vector3.zero;
    }

    //update and check wether player is still on wall
    private void HandleWallRunMovement()
    {
        _wallRunMovementVector.y = _rigidBody.velocity.y / 2;
        _rigidBody.velocity = _wallRunMovementVector;

        if (!Physics.Raycast(transform.position + Vector3.up, _movementWhenWallHit.normalized, _wallDistanceCheck, LayerMask.GetMask(RUNWALL_LAYER)))
        {
            _wasOnWall = true;
            _onWall = false;
            _camera.m_Dutch = 0;
            _lookAtOrigin.transform.Rotate(Vector3.forward, -_currentCameraDutchAngle, Space.Self);
        }
    }
    private void UnCrouch()
    {
        _rigidBody.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        _crouched = false;
    }
    public void Respawn(float respawnAngle)
    {
        _rigidBody.velocity = Vector3.zero;


        Vector3 horizontalRotationAxis = Vector3.up;
        Vector3 verticalRotationAxis = Vector3.right;

        _lookAtOrigin.transform.Rotate(verticalRotationAxis, _totalRotation.y);
        _playerBody.transform.Rotate(horizontalRotationAxis, -_totalRotation.x);

        _totalRotation.y = 0;
        _totalRotation.x = -respawnAngle;
        _totalRotation.y = Math.Clamp(_totalRotation.y, -70, 70);

        _playerBody.transform.Rotate(horizontalRotationAxis, _totalRotation.x);
        _lookAtOrigin.transform.Rotate(verticalRotationAxis, -_totalRotation.y);
    }
    void HandleWallCollision()
    {
        Ray collisionRay = new Ray(transform.position, transform.forward);
        if (!_onWall && Physics.Raycast(collisionRay, Time.deltaTime * _rigidBody.velocity.magnitude,LayerMask.GetMask(RUNWALL_LAYER)))
        {
            _rigidBody.velocity = Vector3.zero;
        }
    }
}
