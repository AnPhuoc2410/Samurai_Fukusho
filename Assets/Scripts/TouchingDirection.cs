using System;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    Rigidbody2D rb;
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float cellDistance = 0.05f;

    public Vector2 wallCheckDirection =>
        gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    CapsuleCollider2D touchCollider2D;
    Animator animator;

    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] cellHits = new RaycastHit2D[5];

    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isOnWall;
    [SerializeField] private bool _isOnCelling;

    public bool IsGrounded
    {
        get { return _isGrounded; }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrouned, value);
        }
    }

    public bool IsOnWall
    {
        get { return _isOnWall; }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    public bool IsOnCelling
    {
        get { return _isOnCelling; }
        private set
        {
            _isOnCelling = value;
            animator.SetBool(AnimationStrings.isOnCelling, value);
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchCollider2D = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        IsGrounded = touchCollider2D.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchCollider2D.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCelling = touchCollider2D.Cast(Vector2.up, castFilter, cellHits, cellDistance) > 0;
    }
}