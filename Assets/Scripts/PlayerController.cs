using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(TouchingDirection), typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D cc;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float airWalkSpeed = 3f;
    [SerializeField] private float jumpForce = 7f;

    private TouchingDirection touchingDirection;
    private Damageable damageable;
    private PlayerHealth playerHealth;

    // --- Defense Buff ---
    [Header("Defense Buff (Auto)")]
    [Tooltip("Giảm damage nhận vào theo phần trăm hoặc số lượng cố định. Chỉ lấy giá trị lớn nhất nếu có nhiều buff.")]
    public bool hasDefenseBuff = false;
    [Tooltip("Nếu true, giảm damage theo phần trăm. Nếu false, giảm damage theo số lượng cố định.")]
    public bool defenseByPercent = true;
    [Tooltip("Giá trị giảm damage (phần trăm hoặc số lượng cố định)")]
    public float defenseValue = 0f;
    [Tooltip("SFX khi bắt đầu buff giáp (tùy chọn)")]
    public AudioClip defenseBuffSFX;
    [Tooltip("SFX khi kết thúc buff giáp (tùy chọn)")]
    public AudioClip defenseBuffEndSFX;
    private AudioSource audioSource;

    // --- Damage Buff ---
    [Header("Damage Buff (Auto)")]
    [Tooltip("Tăng damage gây ra theo phần trăm hoặc số lượng cố định. Chỉ lấy giá trị lớn nhất nếu có nhiều buff.")]
    public bool hasDamageBuff = false;
    [Tooltip("Nếu true, tăng damage theo phần trăm. Nếu false, tăng damage theo số lượng cố định.")]
    public bool damageByPercent = true;
    [Tooltip("Giá trị tăng damage (phần trăm hoặc số lượng cố định)")]
    public float damageBuffValue = 0f;
    [Tooltip("SFX khi bắt đầu buff damage (tùy chọn)")]
    public AudioClip damageBuffSFX;
    [Tooltip("SFX khi kết thúc buff damage (tùy chọn)")]
    public AudioClip damageBuffEndSFX;

    private Vector2 moveInput;
    private bool isFacingRight = true;
    private bool isMoving = false;
    private bool isRunning = false;

    public bool CanMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    private float MoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (isMoving && !touchingDirection.IsOnWall)
                {
                    if (!touchingDirection.IsGrounded)
                    {
                        return airWalkSpeed;
                    }

                    return isRunning ? runSpeed : walkSpeed;
                }
            }

            return 0f;
        }
    }

    public bool LockVelocity
    {
        get => animator.GetBool(AnimationStrings.lockVelocity);
        set => animator.SetBool(AnimationStrings.lockVelocity, value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
        playerHealth = GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        damageable.damageableDeath.AddListener(OnDeath);
    }
    private void OnDisable()
    {
        damageable.damageableDeath.RemoveListener(OnDeath);
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            rb.linearVelocity = new Vector2(moveInput.x * MoveSpeed, rb.linearVelocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            isMoving = moveInput != Vector2.zero;

            if (moveInput.x != 0)
            {
                SetFacingDirection(moveInput.x);
            }

            animator.SetBool(AnimationStrings.isMoving, isMoving);
        }
        else
        {
            cc.offset = new Vector2(0, 0.2f); // Disable collider offset when dead
            isMoving = false;
        }
    }

    private void SetFacingDirection(float directionX)
    {
        bool shouldFaceRight = directionX > 0;

        if (isFacingRight != shouldFaceRight)
        {
            isFacingRight = shouldFaceRight;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (shouldFaceRight ? 1 : -1);
            transform.localScale = scale;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.performed;
        animator.SetBool(AnimationStrings.isRunning, isRunning);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    // Gọi khi bị enemy đánh
    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);

        // --- Áp dụng giảm damage nếu có buff ---
        int finalDamage = damage;
        if (hasDefenseBuff)
        {
            if (defenseByPercent)
            {
                finalDamage = Mathf.CeilToInt(damage * (1f - Mathf.Clamp01(defenseValue / 100f)));
            }
            else
            {
                finalDamage = Mathf.Max(0, damage - Mathf.RoundToInt(defenseValue));
            }
        }

        // Trừ máu
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(finalDamage);
        }
    }
    public void OnDeath()
    {
        cc.offset = new Vector2(0, 0.2f); // Disable collider when dead
        rb.linearVelocity = Vector2.zero;
    }

    // --- API cho ArmorPickup gọi ---
    public void ApplyDefenseBuff(bool byPercent, float value, float duration = 0f, AudioClip startSFX = null, AudioClip endSFX = null)
    {
        // Nếu đã có buff, chỉ lấy giá trị lớn nhất
        if (!hasDefenseBuff || value > defenseValue)
        {
            hasDefenseBuff = true;
            defenseByPercent = byPercent;
            defenseValue = value;
            if (startSFX != null && audioSource != null)
                audioSource.PlayOneShot(startSFX);
            if (duration > 0f)
                StartCoroutine(RemoveDefenseBuffAfter(duration, endSFX));
        }
    }
    public void RemoveDefenseBuff(AudioClip endSFX = null)
    {
        hasDefenseBuff = false;
        defenseValue = 0f;
        if (endSFX != null && audioSource != null)
            audioSource.PlayOneShot(endSFX);
    }
    private System.Collections.IEnumerator RemoveDefenseBuffAfter(float duration, AudioClip endSFX)
    {
        yield return new WaitForSeconds(duration);
        RemoveDefenseBuff(endSFX);
    }

    // --- API cho DamagePickup gọi ---
    public void ApplyDamageBuff(bool byPercent, float value, float duration = 0f, AudioClip startSFX = null, AudioClip endSFX = null)
    {
        // Nếu đã có buff, chỉ lấy giá trị lớn nhất
        if (!hasDamageBuff || value > damageBuffValue)
        {
            hasDamageBuff = true;
            damageByPercent = byPercent;
            damageBuffValue = value;
            if (startSFX != null && audioSource != null)
                audioSource.PlayOneShot(startSFX);
            if (duration > 0f)
                StartCoroutine(RemoveDamageBuffAfter(duration, endSFX));
        }
    }
    public void RemoveDamageBuff(AudioClip endSFX = null)
    {
        hasDamageBuff = false;
        damageBuffValue = 0f;
        if (endSFX != null && audioSource != null)
            audioSource.PlayOneShot(endSFX);
    }
    private System.Collections.IEnumerator RemoveDamageBuffAfter(float duration, AudioClip endSFX)
    {
        yield return new WaitForSeconds(duration);
        RemoveDamageBuff(endSFX);
    }

    // --- Hook vào chỗ tính damage khi player tấn công ---
    public int GetModifiedDamage(int baseDamage)
    {
        if (hasDamageBuff)
        {
            if (damageByPercent)
                return Mathf.CeilToInt(baseDamage * (1f + Mathf.Clamp01(damageBuffValue / 100f)));
            else
                return baseDamage + Mathf.RoundToInt(damageBuffValue);
        }
        return baseDamage;
    }
}
