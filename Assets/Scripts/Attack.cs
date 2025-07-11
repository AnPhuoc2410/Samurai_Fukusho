using UnityEngine;

public class Attack : MonoBehaviour
{
    Collider2D attackCollider;
    [SerializeField] private int attackDamage = 10;
    public Vector2 knockback = new Vector2(0, 0);

    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            // Log damage thực tế của player nếu có
            var playerController = transform.root.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.LogCurrentDamage(attackDamage);
            }

            if (gotHit)
            {
                Debug.Log($"{collision.name} was hit for {attackDamage} damage.");
            }
        }
    }
}
