using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour

{
    public Slider fillBar;
    public TextMeshProUGUI valueText;

    public void UpdateBar(int currentHealth, int maxHealth)
    {
        fillBar.value = (float)currentHealth / (float)maxHealth;
        valueText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
