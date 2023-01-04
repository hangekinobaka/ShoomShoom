using UnityEngine;
using UnityEngine.InputSystem;

public enum GroundType
{
    None, // When the ground type is none, there is no ground under the Player's feet.
    Normal,
    Water
}
public class CharacterController2D : MonoBehaviour
{
    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

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

    Rigidbody2D _controllerRigidbody;
    Collider2D _controllerCollider;
    LayerMask _normalGroundMask;
    LayerMask _waterGroundMask;
    GroundType _groundType;

    Vector2 _movementInput;
    bool _jumpInput;

    Vector2 _prevVelocity;
    bool _isFlipped;
    bool _isJumping;
    bool _isFalling;
    int _localJumpCount;

    public bool CanMove { get; set; }

    void Start()
    {
#if UNITY_EDITOR
        if (Keyboard.current == null)
        {
            var playerSettings = new UnityEditor.SerializedObject(Resources.FindObjectsOfTypeAll<UnityEditor.PlayerSettings>()[0]);
            var newInputSystemProperty = playerSettings.FindProperty("enableNativePlatformBackendsForNewInputSystem");
            bool newInputSystemEnabled = newInputSystemProperty != null ? newInputSystemProperty.boolValue : false;

            if (newInputSystemEnabled)
            {
                var msg = "New Input System backend is enabled but it requires you to restart Unity, otherwise the player controls won't work. Do you want to restart now?";
                if (UnityEditor.EditorUtility.DisplayDialog("Warning", msg, "Yes", "No"))
                {
                    UnityEditor.EditorApplication.ExitPlaymode();
                    var dataPath = Application.dataPath;
                    var projectPath = dataPath.Substring(0, dataPath.Length - 7);
                    UnityEditor.EditorApplication.OpenProject(projectPath);
                }
            }
        }
#endif
        // Get comps
        _controllerRigidbody = GetComponent<Rigidbody2D>();
        _controllerCollider = GetComponent<Collider2D>();
        // Get layer masks
        _normalGroundMask = LayerMask.GetMask("Ground");
        _waterGroundMask = LayerMask.GetMask("GroundWater");

        CanMove = true;
        _localJumpCount = _jumpCount;
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (!CanMove || keyboard == null)
            return;

        // Horizontal movement
        float moveHorizontal = 0.0f;

        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
            moveHorizontal = -1.0f;
        else if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
            moveHorizontal = 1.0f;

        _movementInput = new Vector2(moveHorizontal, 0);

        // Jumping input
        if (keyboard.spaceKey.wasPressedThisFrame)
            // Check if we have ran out of the jump count
            if (_localJumpCount > 0) _jumpInput = true;
    }

    void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateJump();
        UpdateGravityScale();

        _prevVelocity = _controllerRigidbody.velocity;
    }

    private void UpdateGrounding()
    {
        // Use character collider to check if touching ground layers
        if (_controllerCollider.IsTouchingLayers(_normalGroundMask))
            _groundType = GroundType.Normal;
        else if (_controllerCollider.IsTouchingLayers(_waterGroundMask))
            _groundType = GroundType.Water;
        else
            _groundType = GroundType.None;

    }
    private void UpdateVelocity()
    {
        Vector2 velocity = _controllerRigidbody.velocity;

        // Apply acceleration directly as we'll want to clamp
        // prior to assigning back to the body.
        velocity += _movementInput * _acceleration * Time.fixedDeltaTime;

        // We've consumed the movement, reset it.
        _movementInput = Vector2.zero;

        // Clamp horizontal speed.
        velocity.x = Mathf.Clamp(velocity.x, -_maxSpeed, _maxSpeed);

        // Assign back to the body.
        _controllerRigidbody.velocity = velocity;

    }

    private void UpdateJump()
    {
        // Set falling flag
        if (_isJumping && _controllerRigidbody.velocity.y < 0)
            _isFalling = true;

        // Jump
        if (_jumpInput)
        {
            // Jump using impulse force
            _controllerRigidbody.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);

            // We've consumed the jump, reset it.
            _jumpInput = false;
            _localJumpCount--;

            // Set jumping flag
            _isJumping = true;
        }
        // Landed
        else if (_isJumping && _isFalling && _groundType != GroundType.None)
        {
            // Reset the jump count
            _localJumpCount = _jumpCount;

            // Since collision with ground stops rigidbody, reset velocity
            if (_resetSpeedOnLand)
            {
                _prevVelocity.y = _controllerRigidbody.velocity.y;
                _controllerRigidbody.velocity = _prevVelocity;
            }

            // Reset jumping flags
            _isJumping = false;
            _isFalling = false;

        }
    }

    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (_controllerRigidbody.velocity.x > _minFlipSpeed && _isFlipped)
        {
            _isFlipped = false;
            transform.localScale = Vector3.one;
        }
        else if (_controllerRigidbody.velocity.x < -_minFlipSpeed && !_isFlipped)
        {
            _isFlipped = true;
            transform.localScale = flippedScale;
        }
    }


    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = _groundedGravityScale;

        // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
        gravityScale = _controllerRigidbody.velocity.y > 0.0f ? _jumpGravityScale : _fallGravityScale;

        _controllerRigidbody.gravityScale = gravityScale;
    }

}
