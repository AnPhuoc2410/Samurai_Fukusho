using UnityEngine;

public class Damageable : MonoBehaviour
{

    [SerializeField]
    private int _maxHealth = 100;
    private int _currentHealth = 100;
    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceHit = 0f;
    private float invincibleTimer = 0.25f;

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
                IsAvlive = false;
            }
        }
    }
    public bool IsAvlive
    {
        get { return _isAlive; }
        private set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive: " + value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

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

        Hit(10);
    }
    public void Hit(int damage)
    {
        if (IsAvlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
        }
    }

}
