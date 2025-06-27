using UnityEngine;

public enum WalkDirection { Left, Right }

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class Goblin : MonoBehaviour
{
    public float speed = 3f;
    public float walkStopRate = 0.1f;
    Rigidbody2D rb;
    public DetectionZone attackZone;

    private Vector2 walkVector = Vector2.right;
    TouchingDirection touchingDirection;
    Animator animator;

    private WalkDirection _walkDirection = WalkDirection.Left;

    private WalkDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Flip the scale
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (value == WalkDirection.Right ? 1 : -1);
                transform.localScale = scale;

                // Change walk vector
                walkVector = (value == WalkDirection.Right) ? Vector2.right : Vector2.left;
            }

            _walkDirection = value;
        }
    }
    private bool _hasTarget = false;

    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }
    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        WalkDirection = _walkDirection;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    void FixedUpdate()
    {
        if (touchingDirection.IsGrounded && touchingDirection.IsOnWall)
        {
            // Change direction when hitting a wall
            WalkDirection = WalkDirection == WalkDirection.Right ? WalkDirection.Left : WalkDirection.Right;
        }
        if (CanMove)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(walkVector.x * speed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x,0,walkStopRate), rb.linearVelocity.y);
        }

    }
}
