using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Witch : MonoBehaviour
{
    public DetectionZone detectionZone;
    public List<Transform> waypoints;
    public float flySpeed = 2f;
    public float chaseSpeed = 3f; // Add chase speed variable
    public float waypointReachedDistance = 0.1f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;
    Transform nextWaypoint;
    int waypointNum = 0;
    public bool _hasTarget = false;

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
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                if (HasTarget)
                {
                    if (CanAttack())
                    {
                        Attack();
                    }
                    else
                    {
                        Chase(); // Add chase behavior
                    }
                }
                else
                {
                    Flight();
                }
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
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
            // Only chase horizontally, ignore vertical difference (no jumping)
            Vector2 directionToPlayer = new Vector2(
                player.position.x - transform.position.x,
                0
            ).normalized;

            // Move towards player with chase speed (horizontal only)
            rb.linearVelocity = new Vector2(directionToPlayer.x * chaseSpeed, rb.linearVelocity.y);

            // Update witch's facing direction
            UpdateDirection();
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
    }
}
