using UnityEngine;
using System.Collections.Generic;

public class CheatCode : MonoBehaviour
{
    [Header("Cheat Code Settings")]
    [Tooltip("The key sequence to activate cheat (ILOVEYOU)")]
    private string cheatSequence = "ILOVEYOU";
    [Tooltip("Time window to complete the sequence (in seconds)")]
    public float inputWindow = 3f;
    
    [Header("Buff Values")]
    [Tooltip("Speed buff percentage")]
    public float speedBuffValue = 50f;
    [Tooltip("Damage buff percentage")]
    public float damageBuffValue = 100f;
    [Tooltip("Defense buff percentage")]
    public float defenseBuffValue = 50f;
    [Tooltip("Jump buff percentage")]
    public float jumpBuffValue = 50f;
    [Tooltip("Duration for all buffs (0 = permanent)")]
    public float buffDuration = 30f;
    
    [Header("Item Sprites")]
    [Tooltip("Sprite for speed boost item (optional - will create default if null)")]
    public Sprite speedBoostSprite;
    [Tooltip("Sprite for damage boost item (optional - will create default if null)")]
    public Sprite damageBoostSprite;
    [Tooltip("Sprite for armor boost item (optional - will create default if null)")]
    public Sprite armorBoostSprite;
    [Tooltip("Sprite for jump boost item (optional - will create default if null)")]
    public Sprite jumpBoostSprite;
    
    [Header("Audio")]
    [Tooltip("Sound effect when cheat is activated")]
    public AudioClip cheatActivatedSFX;
    
    private Queue<char> inputBuffer = new Queue<char>();
    private float lastInputTime;
    private PlayerController playerController;
    private AudioSource audioSource;
    
    // Singleton pattern for easy access
    public static CheatCode Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void Start()
    {
        FindPlayerController();
    }
    
    private void Update()
    {
        // Find player controller if not found yet
        if (playerController == null)
        {
            FindPlayerController();
        }
        
        // Clear buffer if too much time has passed
        if (Time.time - lastInputTime > inputWindow)
        {
            inputBuffer.Clear();
        }
        
        // Check for keyboard input
        CheckKeyboardInput();
    }
    
    private void FindPlayerController()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }    
    private void CheckKeyboardInput()
    {
        // Check each letter in the alphabet
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                // Convert KeyCode to char (only for letter keys)
                char inputChar = GetCharFromKeyCode(keyCode);
                if (inputChar != '\0')
                {
                    ProcessInput(inputChar);
                }
            }
        }
    }
    
    private char GetCharFromKeyCode(KeyCode keyCode)
    {
        // Convert KeyCode to uppercase char for letters A-Z
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
            return (char)('A' + (keyCode - KeyCode.A));
        }
        return '\0'; // Return null character if not a letter
    }
    
    private void ProcessInput(char inputChar)
    {
        lastInputTime = Time.time;
        
        // Add to buffer
        inputBuffer.Enqueue(inputChar);
        
        // Keep buffer size manageable
        while (inputBuffer.Count > cheatSequence.Length)
        {
            inputBuffer.Dequeue();
        }
        
        // Check if current buffer matches cheat sequence
        if (inputBuffer.Count == cheatSequence.Length)
        {
            string currentSequence = string.Join("", inputBuffer.ToArray());
            if (currentSequence == cheatSequence)
            {
                ActivateCheat();
                inputBuffer.Clear(); // Clear buffer after successful activation
            }
        }
    }
    
    private void ActivateCheat()
    {
        if (playerController == null)
        {
            Debug.LogWarning("[CheatCode] Player controller not found! Cannot apply buffs.");
            return;
        }
        
        Debug.Log("[CheatCode] CHEAT ACTIVATED! Applying all buffs to player...");
        
        // Apply all buffs to the player
        playerController.ApplySpeedBuff(true, speedBuffValue, buffDuration, cheatActivatedSFX);
        playerController.ApplyDamageBuff(true, damageBuffValue, buffDuration);
        playerController.ApplyDefenseBuff(true, defenseBuffValue, buffDuration);
        playerController.ApplyJumpBuff(true, jumpBuffValue, buffDuration);
        
        // Play cheat activation sound
        if (cheatActivatedSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(cheatActivatedSFX);
        }
        
        // Add items to inventory for visual feedback
        AddCheatItemsToInventory();
       
    }
    
    private void AddCheatItemsToInventory()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.AddItem("Cheat Speed Boost", speedBoostSprite, ItemType.Misc);
            inventory.AddItem("Cheat Damage Boost", damageBoostSprite, ItemType.Weapon);
            inventory.AddItem("Cheat Armor Boost", armorBoostSprite, ItemType.Misc);
            inventory.AddItem("Cheat Jump Boost", jumpBoostSprite, ItemType.Misc);
        }
    }
    
    public void TriggerCheat()
    {
        ActivateCheat();
    }
    
    public string GetCurrentInput()
    {
        return string.Join("", inputBuffer.ToArray());
    }
}