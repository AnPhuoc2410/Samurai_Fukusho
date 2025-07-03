using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 20; // Amount of health to restore
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    private void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime; // Spin the health pickup
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            if (!damageable.Heal(healthAmount)) return;
            Destroy(gameObject); // Destroy the health pickup after use
        }
    }
}
