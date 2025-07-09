using UnityEngine;

public enum WalkDirection { Left, Right }

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class Goblin : MonoBehaviour
{
    public float walkAccel = 3f;
    public float maxSpeed = 5f;
    public float walkStopRate = 0.1f;
    Rigidbody2D rb;
    CapsuleCollider2D cc;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    private Vector2 walkVector = Vector2.right;
    TouchingDirection touchingDirection;
    Animator animator;
    Damageable damageable;

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

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.AttackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.AttackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        WalkDirection = _walkDirection;
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }
    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        damageable.damageableDeath.RemoveListener(OnDeath);
    }

    private void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (touchingDirection.IsGrounded && touchingDirection.IsOnWall)
        {
            // Change direction when hitting a wall
            WalkDirection = WalkDirection == WalkDirection.Right ? WalkDirection.Left : WalkDirection.Right;
        }
        if (!damageable.LockVelocity)
        {
            if (CanMove)
            {
                float xVelocity = Mathf.Clamp(rb.linearVelocity.x + (walkAccel * walkVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed);
                rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
            }
        }

    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
    public void OnDeath()
    {
        cc.offset = new Vector2(0, 0.327f);
        rb.linearVelocity = Vector2.zero;
    }

    public void OnCliffDetected()
    {
        if (touchingDirection.IsGrounded)
        {
            WalkDirection = WalkDirection == WalkDirection.Right ? WalkDirection.Left : WalkDirection.Right;
        }
    }
}
