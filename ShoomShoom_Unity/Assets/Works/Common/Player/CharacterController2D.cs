using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GroundType
{
    None, // When the ground type is none, there is no ground under the Player's feet.
    Normal,
    Water
}
public enum MovingDirection
{
    Left,
    Right
}

public enum PlayerState
{
    Idle,
    TurnLeft,
    TurnRight,
    Run,
    Jump,
    DoubleJump,
    Fall,
    Land
}

public class CharacterController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _characterTransform;

    [Header("Movement")]
    [SerializeField] float _acceleration = 0.0f;
    [SerializeField] float _maxSpeed = 0.0f;
    [SerializeField] float _jumpForce = 0.0f;
    [SerializeField] float _minFlipSpeed = 0.1f;
    [SerializeField] float _jumpGravityScale = 1.0f;
    [SerializeField] float _fallGravityScale = 1.0f;
    [SerializeField] float _groundedGravityScale = 1.0f;
    [SerializeField] bool _resetSpeedOnLand = false;
    [SerializeField] int _jumpCount = 2;
    Vector2 _velocity;
    Vector3 _normalScale = Vector3.one;
    Vector3 _flippedScale = new Vector3(-1, 1, 1);
    public float CurRunSpeed => Mathf.Abs(_velocity.x);
    public float MaxSpeed => _maxSpeed;

    [Header("Viewport")]
    [SerializeField] Transform _focalPoint;
    [SerializeField] float _focalMoveDuration = 1f;
    Vector3 _normalFocalPos = new Vector3(3, 0, 0);
    Vector3 _flippedFocalPos = new Vector3(-3, 0, 0);

    [Header("Sprint")]
    [SerializeField] float _sprintMultiplier = 1.5f;
    [SerializeField] bool _isSprinting = false;
    float _curAcceleration;
    float _curMaxSpeed;

    Rigidbody2D _controllerRigidbody;
    Collider2D _controllerCollider;
    LayerMask _normalGroundMask;
    LayerMask _waterGroundMask;
    GroundType _groundType;
    PlayerInputAction _playerInputAction;
    public GroundType CurGroundType => _groundType;

    Vector2 _movementInput;
    bool _jumpInput;

    Vector2 _prevVelocity;
    bool _prepareJump;
    bool _isJumping;
    bool _isFalling;
    bool _isLanding;
    bool _isGrounded;
    int _localJumpCount;

    MovingDirection _dir = MovingDirection.Right;
    public ReactProps<PlayerState> CurPlayerState = new ReactProps<PlayerState>(PlayerState.Idle);


    private void Awake()
    {
        // Get comps
        _controllerRigidbody = GetComponent<Rigidbody2D>();
        _controllerCollider = GetComponent<Collider2D>();
        // Get layer masks
        _normalGroundMask = LayerMask.GetMask("Ground");
        _waterGroundMask = LayerMask.GetMask("GroundWater");
        // Init basic vals
        _normalScale = _characterTransform.localScale;
        _flippedScale = _normalScale;
        _flippedScale.x = -_flippedScale.x;
        _prepareJump = false;
        _isLanding = false;

        CurPlayerState.SetState(PlayerState.Idle);
    }

    void Start()
    {
        // Init vals
        _playerInputAction = new PlayerInputAction();
        _normalFocalPos = _focalPoint.localPosition;
        _flippedFocalPos = _normalFocalPos;
        _flippedFocalPos.x = -_flippedFocalPos.x;
        _localJumpCount = _jumpCount;

        // Input system
        _playerInputAction.Normal.Enable();
        _playerInputAction.Normal.Move.performed += MoveInputHandler;
        _playerInputAction.Normal.Move.canceled += MoveInputHandler;
        _playerInputAction.Normal.Jump.performed += JumpInputHandler;
        _playerInputAction.Normal.Sprint.performed += SprintInputHandler;
        _playerInputAction.Normal.Sprint.canceled += SprintInputHandler;
    }

    private void OnDisable()
    {
        _playerInputAction.Normal.Move.performed -= MoveInputHandler;
        _playerInputAction.Normal.Move.canceled -= MoveInputHandler;
        _playerInputAction.Normal.Jump.performed -= JumpInputHandler;
        _playerInputAction.Normal.Sprint.performed += SprintInputHandler;
        _playerInputAction.Normal.Sprint.canceled += SprintInputHandler;
        _playerInputAction.Normal.Disable();
    }

    void FixedUpdate()
    {
        if (_prepareJump || _isLanding) return;

        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateFall();
        UpdateJump();
        UpdateLand();
        UpdateGravityScale();
        UpdateState();

        _prevVelocity = _controllerRigidbody.velocity;
    }

    void MoveInputHandler(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        _movementInput = new Vector2(inputVector.x, 0);
    }
    void JumpInputHandler(InputAction.CallbackContext context)
    {
        // Check if we have ran out of the jump count
        if (_localJumpCount > 0) _jumpInput = true;
    }
    void SprintInputHandler(InputAction.CallbackContext context)
    {
        // If shift key is being held, enable the sprint mode
        if (context.performed)
            _isSprinting = true;
        else if (context.canceled)
            _isSprinting = false;

    }

    private void UpdateGrounding()
    {
        _isGrounded = true;
        // Use character collider to check if touching ground layers
        if (_controllerCollider.IsTouchingLayers(_normalGroundMask))
            _groundType = GroundType.Normal;
        else if (_controllerCollider.IsTouchingLayers(_waterGroundMask))
            _groundType = GroundType.Water;
        else
        {
            _isGrounded = false;
            _groundType = GroundType.None;
        }

    }
    private void UpdateVelocity()
    {
        _velocity = _controllerRigidbody.velocity;

        // Handle sprint 
        if (_isSprinting)
        {
            _curAcceleration = _acceleration * _sprintMultiplier;
            _curMaxSpeed = _maxSpeed * _sprintMultiplier;
        }
        else
        {
            _curAcceleration = _acceleration;
            _curMaxSpeed = _maxSpeed;
        }

        // Apply acceleration directly as we'll want to clamp
        // prior to assigning back to the body.
        _velocity += _movementInput * _curAcceleration * Time.fixedDeltaTime;

        // Clamp horizontal speed.
        _velocity.x = Mathf.Clamp(_velocity.x, -_curMaxSpeed, _curMaxSpeed);

        // Assign back to the body.
        _controllerRigidbody.velocity = _velocity;
    }

    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (_dir == MovingDirection.Left && _velocity.x > _minFlipSpeed)
        {
            _dir = MovingDirection.Right;
            _characterTransform.localScale = _normalScale;
            _focalPoint.DOLocalMove(_normalFocalPos, _focalMoveDuration);
            CurPlayerState.SetState(PlayerState.TurnRight);
        }
        else if (_dir == MovingDirection.Right && _velocity.x < -_minFlipSpeed)
        {
            _dir = MovingDirection.Left;
            _characterTransform.localScale = _flippedScale;
            _focalPoint.DOLocalMove(_flippedFocalPos, _focalMoveDuration);
            CurPlayerState.SetState(PlayerState.TurnLeft);
        }
    }

    private void UpdateFall()
    {
        // Set falling flag
        if (_isFalling != _velocity.y < 0)  // falling state changed
        {
            _isFalling = _velocity.y < 0; // assign the current state

            if (_isFalling)
            {
                CurPlayerState.SetState(PlayerState.Fall);
            }
        }
    }

    private void UpdateJump()
    {
        // Jump
        if (_jumpInput)
        {
            _prepareJump = true;

            if (_localJumpCount >= 2)
            {
                CurPlayerState.SetState(PlayerState.Jump);
            }
            else
            {
                CurPlayerState.SetState(PlayerState.DoubleJump);
            }
        }
    }

    private void UpdateLand()
    {
        // Test if landed
        if (_prevVelocity.y != _velocity.y && _velocity.y == 0)
        {
            // Since collision with ground stops rigidbody, reset velocity
            // (if this function is enabled)
            if (_resetSpeedOnLand)
            {
                _prevVelocity.y = _velocity.y;
                _controllerRigidbody.velocity = _prevVelocity;
                _velocity = _prevVelocity;
            }

            // Reset jumping flags
            _isJumping = false;
            // Reset the jump count
            _localJumpCount = _jumpCount;

            // Set the land state
            CurPlayerState.SetState(PlayerState.Land);
            _isLanding = true;
        }
    }

    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = _groundedGravityScale;

        // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
        gravityScale = _velocity.y > 0.0f ? _jumpGravityScale : _fallGravityScale;

        _controllerRigidbody.gravityScale = gravityScale;
    }

    private void UpdateState()
    {
        if (_velocity.y == 0 && !_isJumping && !_prepareJump && !_isFalling && !_isLanding)
        {
            if (Mathf.Abs(_velocity.x) > 0)
            {
                CurPlayerState.SetState(PlayerState.Run);
            }
            else
            {
                CurPlayerState.SetState(PlayerState.Idle);
            }
        }
    }

    public void Jump()
    {
        // Jump using impulse force
        _controllerRigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);

        // We've consumed the jump, reset it.
        _jumpInput = false;
        _localJumpCount--;

        // Set jumping flag
        _isJumping = true;
        _prepareJump = false;
    }

    public void Landed()
    {
        _isLanding = false;
    }

}
