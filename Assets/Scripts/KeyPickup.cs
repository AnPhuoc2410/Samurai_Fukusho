using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string keyName = "Level1Key"; // Name of the key (unique identifier)
    [SerializeField] private string keyDescription = "A mysterious key that opens doors"; // Description of the key
    [SerializeField] private Vector3 spinRotationSpeed = new Vector3(0, 180, 0); // Spin animation
    
    [Header("Visual Settings")]
    [SerializeField] private bool hasFloatingAnimation = true;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.5f;
    
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Spin the key
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
        
        // Floating animation
        if (hasFloatingAnimation)
        {
            Vector3 pos = startPos;
            pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collision object is the player
        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Get the PlayerInventory instance
            PlayerInventory inventory = PlayerInventory.Instance;
            
            if (inventory != null)
            {
                // Get the sprite from this GameObject's SpriteRenderer
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                Sprite keySprite = spriteRenderer != null ? spriteRenderer.sprite : null;
                
                // Add key to inventory
                if (keySprite != null)
                {
                    inventory.AddItem(keyName, keySprite);
                }
                else
                {
                    inventory.AddKey(keyName);
                }
                
                Debug.Log($"Player picked up key: {keyName}");
                
                // Play pickup sound if there's an AudioSource
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                    // Destroy after sound finishes
                    Destroy(gameObject, audioSource.clip.length);
                }
                else
                {
                    // Destroy immediately if no sound
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("PlayerInventory instance not found! Make sure PlayerInventory is attached to a GameObject in the scene.");
            }
        }
    }


} 