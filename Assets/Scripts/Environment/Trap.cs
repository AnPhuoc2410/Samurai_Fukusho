using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 15;
    public Vector2 knockback = new Vector2(3f, 3f);
    public float trapCooldown = 1f;
    private float lastTriggerTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if enough time has passed since last trigger
        if (Time.time < lastTriggerTime + trapCooldown)
            return;

        // Check if the colliding object is a player
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Damageable damageable = collision.GetComponent<Damageable>();
            if (damageable != null)
            {
                // Calculate knockback direction based on trap position relative to player
                Vector2 deliveredKnockback = (collision.transform.position.x > transform.position.x)
                    ? knockback
                    : new Vector2(-knockback.x, knockback.y);

                bool gotHit = damageable.Hit(damage, deliveredKnockback);

                if (gotHit)
                {
                    Debug.Log($"{collision.name} was hit by trap for {damage} damage.");
                    lastTriggerTime = Time.time;

                    // Optional: Add trap activation effects here
                    // animator.SetTrigger("activate"); // If you have activation animation
                }
            }
        }
    }
}
