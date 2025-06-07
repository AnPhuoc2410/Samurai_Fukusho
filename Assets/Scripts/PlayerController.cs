using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    public float moveSpeed = 5f;
    public bool IsMoving { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Handle any non-physics-based logic here
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    void FixedUpdate()
    {
        _rigidbody.linearVelocity = new Vector2(_movementInput.x * moveSpeed, _movementInput.y * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();

        IsMoving = _movementInput != Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Implement jump logic here
            Debug.Log("Jump action performed");
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Implement firing logic here
            Debug.Log("Fire action performed");
        }
    }
}