using Assets.Scripts.Events;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Text Prefabs")]
    public GameObject damageTextPrefab;
    public GameObject healTextPrefab;

    [Header("UI References")]
    public Canvas gameCanvas;
    public InventoryDisplay InventoryDisplay;

    [Header("Quick Access")]
    [SerializeField] private bool enableInventoryQuickAccess = true;

    private void Awake()
    {
        gameCanvas = FindFirstObjectByType<Canvas>();
        
        // Find InventoryDisplay if not assigned
        if (InventoryDisplay == null)
        {
            InventoryDisplay = FindFirstObjectByType<InventoryDisplay>();
        }
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }
    
    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }

    public void CharacterTookDamage(GameObject character,int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();
        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();
        tmpText.text = healthRestored.ToString();
    }
}
