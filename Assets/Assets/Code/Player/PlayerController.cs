using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Actions (New Input System)")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private InputActionReference meleeAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float airControl = 0.35f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 1.2f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.8f;

    [Header("Combat / Interaction")]
    [SerializeField] private UnityEvent onMelee;
    [SerializeField] private UnityEvent onInteract;

    private CharacterController controller;
    private Vector3 velocity;
    private float pitch;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;

    private InputAction move;
    private InputAction look;
    private InputAction jump;
    private InputAction dash;
    private InputAction melee;
    private InputAction interact;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        move = moveAction.action;
        look = lookAction.action;
        jump = jumpAction.action;
        dash = dashAction.action;
        melee = meleeAction.action;
        interact = interactAction.action;

        move.Enable();
        look.Enable();
        jump.Enable();
        dash.Enable();
        melee.Enable();
        interact.Enable();

        dash.performed += OnDash;
        melee.performed += OnMelee;
        interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        dash.performed -= OnDash;
        melee.performed -= OnMelee;
        interact.performed -= OnInteract;

        move.Disable();
        look.Disable();
        jump.Disable();
        dash.Disable();
        melee.Disable();
        interact.Disable();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    private void HandleLook()
    {
        Vector2 lookInput = look.ReadValue<Vector2>() * lookSensitivity * Time.deltaTime;

        // Yaw
        transform.Rotate(Vector3.up * lookInput.x);

        // Pitch
        pitch -= lookInput.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        Vector2 input = move.ReadValue<Vector2>();
        Vector3 moveVector = (transform.right * input.x + transform.forward * input.y);

        float control = controller.isGrounded ? 1f : airControl;
        Vector3 horizontal = moveVector * moveSpeed * control;

        if (dashTimer > 0f)
        {
            horizontal = dashDirection * dashSpeed;
        }

        controller.Move(horizontal * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && jump.WasPressedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleDash()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTimer > 0f || dashTimer > 0f)
        {
            return;
        }

        Vector2 input = move.ReadValue<Vector2>();
        Vector3 moveVector = (transform.right * input.x + transform.forward * input.y);

        dashDirection = moveVector.sqrMagnitude > 0.01f ? moveVector.normalized : transform.forward;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
    }

    private void OnMelee(InputAction.CallbackContext context)
    {
        onMelee?.Invoke();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        onInteract?.Invoke();
    }
}