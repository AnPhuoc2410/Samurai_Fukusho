using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGate : PressE_ToOpen
{
    [Header("Gate Settings")]
    [SerializeField] private string requiredKeyName = "Level1Key"; // Key needed to open this gate
    [SerializeField] private string targetSceneName = "Scence2"; // Scene to load when gate opens
    [SerializeField] private bool consumeKey = false; // Whether to remove key from inventory after use
    
    [Header("Player Spawn Settings")]
    [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero; // Position to spawn player in target scene
    [SerializeField] private bool useCustomSpawnPosition = false; // Whether to use custom spawn position
    
    [Header("Audio")]
    [SerializeField] private AudioClip lockedSound; // Sound when trying to open without key
    [SerializeField] private AudioClip unlockSound; // Sound when successfully opening gate
    
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private bool hasRequiredKey = false;

    protected override void Start()
    {
        base.Start();
        
        // Get components
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Check for key initially
        CheckForKey();
    }

    protected override void Update()
    {
        // Only allow interaction if player is in range and has required key
        if (player == null || isOpened) return;

        // Check for key state change
        bool previousKeyState = hasRequiredKey;
        CheckForKey();

        float distance = Vector2.Distance(player.transform.position, transform.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            if (hasRequiredKey)
            {
                OnInteract();
            }
            else
            {
                OnInteractLocked();
            }
        }
    }

    private void CheckForKey()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        hasRequiredKey = inventory != null && inventory.HasKey(requiredKeyName);
        
        if (inventory == null)
        {
            Debug.LogWarning("PlayerInventory instance not found when checking for key!");
        }
    }

    protected override void OnInteract()
    {
        if (!hasRequiredKey)
        {
            OnInteractLocked();
            return;
        }

        // Play unlock sound
        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }

        // Remove key from inventory if set to consume
        if (consumeKey)
        {
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                inventory.RemoveKey(requiredKeyName);
            }
        }

        // Call base interaction (opens gate animation)
        base.OnInteract();

        // Load the target scene after a short delay to allow animation/sound
        float delay = unlockSound != null ? unlockSound.length : 0.5f;
        Invoke(nameof(LoadTargetScene), delay);
    }

    private void OnInteractLocked()
    {
        // Play locked sound
        if (audioSource != null && lockedSound != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }

        // Show message to player
        Debug.Log($"Gate is locked! You need the {requiredKeyName} to open this gate.");
        
        // You can add UI message display here
        // For example: UIManager.ShowMessage($"You need the {requiredKeyName}!");
    }

    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"Loading scene: {targetSceneName}");
            
            // Use SceneTransitionManager for proper scene transition with spawn position
            if (useCustomSpawnPosition)
            {
                SceneTransitionManager.Instance.TransitionToScene(targetSceneName, playerSpawnPosition, true);
            }
            else
            {
                SceneTransitionManager.Instance.TransitionToScene(targetSceneName);
            }
        }
        else
        {
            Debug.LogError("Target scene name is not set!");
        }
    }

    /// <summary>
    /// Set the required key name for this gate
    /// </summary>
    /// <param name="keyName">Name of the required key</param>
    public void SetRequiredKey(string keyName)
    {
        requiredKeyName = keyName;
        CheckForKey();
    }

    /// <summary>
    /// Set the target scene to load when gate opens
    /// </summary>
    /// <param name="sceneName">Name of the target scene</param>
    public void SetTargetScene(string sceneName)
    {
        targetSceneName = sceneName;
    }

    /// <summary>
    /// Set the player spawn position for the target scene
    /// </summary>
    /// <param name="spawnPosition">Position to spawn player</param>
    /// <param name="useCustomSpawn">Whether to use the custom spawn position</param>
    public void SetPlayerSpawnPosition(Vector3 spawnPosition, bool useCustomSpawn = true)
    {
        playerSpawnPosition = spawnPosition;
        useCustomSpawnPosition = useCustomSpawn;
    }

    /// <summary>
    /// Check if the gate can be opened (player has required key)
    /// </summary>
    /// <returns>True if gate can be opened, false otherwise</returns>
    public bool CanOpenGate()
    {
        return hasRequiredKey;
    }

    // Override CanInteract to show proper feedback
    public new bool CanInteract()
    {
        if (player == null || isOpened) return false;
        float distance = Vector2.Distance(player.transform.position, transform.position);
        return distance <= interactRange;
    }
} 