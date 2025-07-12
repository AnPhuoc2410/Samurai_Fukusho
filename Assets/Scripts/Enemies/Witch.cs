using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Witch : MonoBehaviour
{
    public DetectionZone detectionZone;
    public List<Transform> waypoints;
    public float flySpeed = 2f;
    public float chaseSpeed = 3f;
    public float waypointReachedDistance = 0.1f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    [Header("Key Drop Settings")]
    [Tooltip("Key prefab sẽ được drop khi Witch chết")]
    public GameObject keyPrefab;
    [Tooltip("Offset vị trí drop key so với vị trí Witch")]
    public Vector2 keyDropOffset = new Vector2(0, 0.5f);

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    Transform nextWaypoint;
    int waypointNum = 0;
    public bool _hasTarget = false;
    private bool hasDroppedKey = false; // Để đảm bảo chỉ drop key một lần

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
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
        private set
        {
            animator.SetBool(AnimationStrings.canMove, value);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
        CanMove = true;
        HasTarget = false;
    }

    void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = detectionZone.detectedColliders.Exists(
            c => c.gameObject.layer == LayerMask.NameToLayer("Player")
        );
    }

    private void FixedUpdate()
    {
        if (!damageable.IsAlive) return;

        if (isKnockedBack)
        {
            knockbackTimer += Time.fixedDeltaTime;
            if (knockbackTimer >= knockbackRecoveryTime)
            {
                isKnockedBack = false;
                knockbackTimer = 0f;
                CanMove = true;
            }

            return;
        }

        if (!CanMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (HasTarget)
        {
            if (CanAttack())
            {
                Attack();
            }
            else
            {
                Chase();
            }
        }
        else
        {
            Flight();
        }
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("attack"); // Trigger attack animation
    }

    private void Flight()
    {
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, nextWaypoint.position);

        rb.linearVelocity = directionToWaypoint * flySpeed;
        UpdateDirection();

        if (distance <= waypointReachedDistance)
        {
            waypointNum++;
            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void Chase()
    {
        // Find the player in the detection zone
        var player = detectionZone.detectedColliders.Find(
            c => c.gameObject.layer == LayerMask.NameToLayer("Player")
        )?.transform;

        if (player != null)
        {
            float distanceX = player.position.x - transform.position.x;

            // Avoid flipping if the player is extremely close (to prevent jitter/glitch)
            if (Mathf.Abs(distanceX) > 0.05f)
            {
                // Only chase horizontally, ignore vertical difference (no jumping)
                Vector2 directionToPlayer = new Vector2(
                    distanceX,
                    0
                ).normalized;

                // Move towards player with chase speed (horizontal only)
                rb.linearVelocity = new Vector2(directionToPlayer.x * chaseSpeed, rb.linearVelocity.y);

                // Update witch's facing direction
                UpdateDirection();
            }
            else
            {
                // Stop horizontal movement and keep facing direction unchanged
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    private void UpdateDirection()
    {
        Vector3 localScale = transform.localScale;

        if (transform.localScale.x > 0)
        {
            if (rb.linearVelocity.x < 0)
            {
                // If the velocity is negative, flip the sprite to face left
                transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            }
        }
        else if (transform.localScale.x < 0)
        {
            if (rb.linearVelocity.x > 0)
            {
                // If the velocity is positive, flip the sprite to face right
                transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            }
        }
    }

    public void OnDeath()
    {
        CanMove = false;
        rb.gravityScale = 2f;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        // Drop key khi Witch chết
        if (!hasDroppedKey && keyPrefab != null)
        {
            Vector3 dropPosition = transform.position + (Vector3)keyDropOffset;
            GameObject droppedKey = Instantiate(keyPrefab, dropPosition, Quaternion.identity);
            hasDroppedKey = true;
            
            Debug.Log("Witch đã drop key tại vị trí: " + dropPosition);
        }
    }

    public float knockbackRecoveryTime = 0.5f;  // Time before witch can move again after being hit
    public float knockbackForce = 5f;  // Force of knockback
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    public void OnHit(int damage, Vector2 knockback)
    {
        // Disable movement temporarily
        CanMove = false;
        isKnockedBack = true;
        knockbackTimer = 0f;

        // Apply knockback force
        Vector2 knockbackDirection = new Vector2(
            Mathf.Sign(knockback.x) * knockbackForce,
            knockbackForce * 0.5f
        );
        rb.linearVelocity = knockbackDirection;

        // Apply damage
        if (damageable != null)
        {
            damageable.Hit(damage, knockback);

            if (!damageable.IsAlive)
            {
                OnDeath();
            }
        }

        // Play hit animation if you have one
        animator.SetTrigger("hit");
        Debug.Log($"{gameObject.name} was hit for {damage} damage.");
    }
}
