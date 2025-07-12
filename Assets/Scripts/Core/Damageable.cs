using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;

    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField]
    private int _currentHealth = 100;
    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;

    // Player-specific fields
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private HealthBarScript healthBar;
    private GameManager gameManager;

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
            
            // Update health bar for player
            if (isPlayer && healthBar != null)
            {
                healthBar.UpdateBar(_currentHealth, _maxHealth);
            }
            
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
                
                // Handle player death
                if (isPlayer)
                {
                    HandlePlayerDeath();
                }
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
    
    private void Start()
    {
        // Initialize player-specific components
        if (isPlayer)
        {
            if (healthBar != null)
            {
                healthBar.UpdateBar(_currentHealth, _maxHealth);
            }
            
            gameManager = FindFirstObjectByType<GameManager>();
        }
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
            // Apply defense buff for player
            int finalDamage = damage;
            if (isPlayer)
            {
                finalDamage = GetModifiedDamageForPlayer(damage);
            }
            
            Health -= finalDamage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, finalDamage);

            return true;
        }
        return false;
    }
    
    private int GetModifiedDamageForPlayer(int baseDamage)
    {
        if (!isPlayer) return baseDamage;
        
        // Get PlayerController component to access defense buff
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null && playerController.hasDefenseBuff)
        {
            if (playerController.defenseByPercent)
            {
                return Mathf.CeilToInt(baseDamage * (1f - Mathf.Clamp01(playerController.defenseValue / 100f)));
            }
            else
            {
                return Mathf.Max(0, baseDamage - Mathf.RoundToInt(playerController.defenseValue));
            }
        }
        
        return baseDamage;
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
    
    // Player-specific methods
    public void TakeDamage(int damage)
    {
        if (isPlayer)
        {
            Health -= damage;
            
            if (Health <= 0)
            {
                Health = 0;
                // IsAlive will be set to false automatically in Health setter
            }
        }
    }
    
    private void HandlePlayerDeath()
    {
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
    
    // Method to set this as a player (can be called from inspector or code)
    public void SetAsPlayer(HealthBarScript playerHealthBar = null)
    {
        isPlayer = true;
        if (playerHealthBar != null)
        {
            healthBar = playerHealthBar;
        }
    }
}
