using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 speed = new Vector2(3f, 0f);
    public float lifetime = 5f;
    public int damage = 20;
    public Vector2 knockback = new Vector2(5f, 5f);
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Set the velocity of the projectile
        rb.linearVelocity = new Vector2(speed.x * transform.localScale.x, speed.y);

        // Destroy the projectile after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log($"{collision.name} was hit for {damage} damage.");
                Destroy(gameObject);
            }
        }
    }
}
