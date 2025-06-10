using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(TouchingDirection))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public bool CanMove => animator.GetBool(AnimationStrings.canMove);

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float airWalkSpeed = 3f;
    [SerializeField] private float jumpForce = 7f;
    TouchingDirection touchingDirection;

    private Vector2 moveInput;
    private bool isFacingRight = true;
    private bool isMoving = false;
    private bool isRunning = false;

    private float MoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (isMoving && !touchingDirection.IsOnWall)
                {
                    if (!touchingDirection.IsGrounded)
                    {
                        return airWalkSpeed; // Air movement speed
                    }

                    if (isRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    return 0f; //Idle
                }
            }
            else
            {
                return 0f; // Cannot move
            }
        }
    }

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * MoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
        animator.SetBool(AnimationStrings.isMoving, isMoving);

        if (moveInput.x != 0)
        {
            SetFacingDirection(moveInput.x);
        }
    }

    private void SetFacingDirection(float directionX)
    {
        bool shouldFaceRight = directionX > 0;
        if (isFacingRight != shouldFaceRight)
        {
            isFacingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (shouldFaceRight ? 1 : -1);
            transform.localScale = scale;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.performed;
        animator.SetBool(AnimationStrings.isRunning, isRunning);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }
}