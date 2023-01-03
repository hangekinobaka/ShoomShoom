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
    readonly Quaternion flippedRotation = new Quaternion(0, 0, 1, 0);

    [Header("Movement")]
    [SerializeField] float acceleration = 0.0f;
    [SerializeField] float maxSpeed = 0.0f;
    [SerializeField] float jumpForce = 0.0f;
    [SerializeField] float minFlipSpeed = 0.1f;
    [SerializeField] float jumpGravityScale = 1.0f;
    [SerializeField] float fallGravityScale = 1.0f;
    [SerializeField] float groundedGravityScale = 1.0f;
    [SerializeField] bool resetSpeedOnLand = false;

    private Rigidbody2D controllerRigidbody;
    private Collider2D controllerCollider;
    private LayerMask normalGroundMask;
    private LayerMask waterGroundMask;
    private GroundType groundType;

    private Vector2 movementInput;
    private bool jumpInput;

    private Vector2 prevVelocity;
    private bool isFlipped;
    private bool isJumping;
    private bool isFalling;

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
        controllerRigidbody = GetComponent<Rigidbody2D>();
        controllerCollider = GetComponent<Collider2D>();
        // Get layer masks
        normalGroundMask = LayerMask.GetMask("Ground");
        waterGroundMask = LayerMask.GetMask("GroundWater");

        CanMove = true;
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

        movementInput = new Vector2(moveHorizontal, 0);

        // Jumping input
        if (!isJumping && keyboard.spaceKey.wasPressedThisFrame)
            jumpInput = true;
    }

    void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateJump();
        UpdateGravityScale();

        prevVelocity = controllerRigidbody.velocity;
    }

    private void UpdateGrounding()
    {
        // Use character collider to check if touching ground layers
        if (controllerCollider.IsTouchingLayers(normalGroundMask))
            groundType = GroundType.Normal;
        else if (controllerCollider.IsTouchingLayers(waterGroundMask))
            groundType = GroundType.Water;
        else
            groundType = GroundType.None;

    }
    private void UpdateVelocity()
    {
        Vector2 velocity = controllerRigidbody.velocity;

        // Apply acceleration directly as we'll want to clamp
        // prior to assigning back to the body.
        velocity += movementInput * acceleration * Time.fixedDeltaTime;

        // We've consumed the movement, reset it.
        movementInput = Vector2.zero;

        // Clamp horizontal speed.
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);

        // Assign back to the body.
        controllerRigidbody.velocity = velocity;

    }

    private void UpdateJump()
    {
        // Set falling flag
        if (isJumping && controllerRigidbody.velocity.y < 0)
            isFalling = true;

        // Jump
        if (jumpInput)
        {
            // Jump using impulse force
            controllerRigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);


            // We've consumed the jump, reset it.
            jumpInput = false;

            // Set jumping flag
            isJumping = true;
        }
        // Landed
        else if (isJumping && isFalling && groundType != GroundType.None)
        {
            // Since collision with ground stops rigidbody, reset velocity
            if (resetSpeedOnLand)
            {
                prevVelocity.y = controllerRigidbody.velocity.y;
                controllerRigidbody.velocity = prevVelocity;
            }

            // Reset jumping flags
            isJumping = false;
            isFalling = false;

        }
    }

    private void UpdateDirection()
    {
        // Use scale to flip character depending on direction
        if (controllerRigidbody.velocity.x > minFlipSpeed && isFlipped)
        {
            isFlipped = false;
            transform.localScale = Vector3.one;
        }
        else if (controllerRigidbody.velocity.x < -minFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            transform.localScale = flippedScale;
        }
    }


    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = groundedGravityScale;

        // If not grounded then set the gravity scale according to upwards (jump) or downwards (falling) motion.
        gravityScale = controllerRigidbody.velocity.y > 0.0f ? jumpGravityScale : fallGravityScale;

        controllerRigidbody.gravityScale = gravityScale;
    }

}
