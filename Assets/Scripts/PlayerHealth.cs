using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int currentHealth;

    public HealthBarScript healthBar;

    public UnityEvent Ondeath;

    private void OnEnable()
    {
        Ondeath.AddListener(Death);
    }

    private void OnDisable()
    {
        Ondeath.RemoveListener(Death);
    }
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.UpdateBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Ondeath.Invoke();
        }
        healthBar.UpdateBar(currentHealth, maxHealth);

    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
