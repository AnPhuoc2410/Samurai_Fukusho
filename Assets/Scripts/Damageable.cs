using UnityEngine;

public class Damageable : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField]
    private bool _isAlive = true;
    private int _currentHealth;
    [SerializeField]
    private bool isInvincible =  false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public int MaxHealth { get { return _maxHealth; } private set { _maxHealth = value; } }

    public int Health
    {
        get { return _currentHealth; }
        private set
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
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        Health = MaxHealth;
    }
    private void Update()
    {
        if (isInvincible)
        {
            if(timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
        Hit(10);
    }
    public void Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            
        }
    }
}
