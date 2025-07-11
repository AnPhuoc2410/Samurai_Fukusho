using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;

    [SerializeField]
    private int _maxHealth = 100;
    private int _currentHealth = 100;
    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;

    public float timeSinceHit = 0f;
    public float invincibleTimer = 0.25f;

    Animator animator;

    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
        }
    }
    public int Health
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0)
            {
                IsAlive = false;
            }
        }
    }

    public bool IsAlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive: " + value);
            if (!value)
            {
                damageableDeath?.Invoke();
                animator.SetBool(AnimationStrings.isAlive, false);
                LockVelocity = true;
            }
            else
            {
                LockVelocity = false;
            }
        }
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
    private void Awake()
    {
        animator = GetComponent<Animator>();
        _currentHealth = _maxHealth;
    }
    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invincibleTimer)
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
    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }
        return false;
    }
    public bool Heal(int amount)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, amount);
            Health += actualHeal;
            CharacterEvents.characterHealed(gameObject, actualHeal);
            return true; // Healing was successful  

        }
        return false; // Healing failed  
    }

}
