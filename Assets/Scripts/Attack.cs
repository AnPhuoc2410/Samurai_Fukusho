using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.Hit(attackDamage);
            Debug.Log("Attacked " + collision.name + " for " + attackDamage + " damage.");
        }
    }
}

