using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageReceived;
    [SerializeField] private Animator animator;
    [SerializeField] private int _maxHealth = 100;
    public int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
        }
    }
    [SerializeField] private int _health = 100;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
                _health = 0; // Ensure health does not go below zero
            }
        }
    }
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool isInvincible = false;
    public bool IsHit
    {
        get
        {
            return animator.GetBool(AnimationStrings.isHit);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isHit, value);
        }
    }
    private float timeSinceHit = 0f;
    private float invincibilityTime = 0.25f;

    public bool IsAlive
    {
        get => _isAlive;
        private set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    public bool LockVelocity
    {
        get => animator.GetBool(AnimationStrings.lockVelocity);
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0f;
            }
            else
            {
                timeSinceHit += Time.deltaTime;
            }
        }
    }

    public bool Hit(int damage, Vector2 knockback = default)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageReceived?.Invoke(damage, knockback);
            return true;
        }
        return false;
    }

}
