using System.Collections.Generic;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public DetectionZone detectionZone;
    public List<Transform> waypoints;
    public float flySpeed = 2f;
    public float waypointReachedDistance = 0.1f;

    // Add these new fields
    public ProjectileLauncher projectileLauncher;
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
                if (HasTarget && CanAttack())
                {
                    Attack();
                }
                else if (!HasTarget)
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

        // Launch projectile (this should be called from animation event or after a delay)
        if (projectileLauncher != null)
        {
            projectileLauncher.LaunchProjectile();
        }
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
