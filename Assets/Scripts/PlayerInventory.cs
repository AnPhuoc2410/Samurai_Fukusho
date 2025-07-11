using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance 
    { 
        get 
        { 
            if (instance == null)
            {
                instance = FindFirstObjectByType<PlayerInventory>();
            }
            return instance; 
        } 
    }

    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    [Header("Events")]
    public UnityEvent OnInventoryChanged;

    private void Awake()
    {
        // Singleton pattern - ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Add an item to the player's inventory
    /// </summary>
    public void AddItem(string itemName, Sprite sprite)
    {
        InventoryItem newItem = new InventoryItem(itemName, sprite);
        items.Add(newItem);
        OnInventoryChanged?.Invoke();
        Debug.Log($"Added item: {itemName}");
    }

    /// <summary>
    /// Add an item to the player's inventory with specific type
    /// </summary>
    public void AddItem(string itemName, Sprite sprite, ItemType itemType)
    {
        InventoryItem newItem = new InventoryItem(itemName, sprite, itemType);
        items.Add(newItem);
        OnInventoryChanged?.Invoke();
        Debug.Log($"Added {itemType}: {itemName}");
    }

    /// <summary>
    /// Add a key to the player's inventory
    /// </summary>
    public void AddKey(string keyName)
    {
        // Check if key already exists
        bool keyExists = false;
        foreach (var item in items)
        {
            if (item.itemName == keyName && item.itemType == ItemType.Key)
            {
                keyExists = true;
                break;
            }
        }

        if (!keyExists)
        {
            InventoryItem keyItem = new InventoryItem(keyName, null); // No sprite for keys added via AddKey
            items.Add(keyItem);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Added key: {keyName}");
        }
    }

    /// <summary>
    /// Check if player has a specific key
    /// </summary>
    public bool HasKey(string keyName)
    {
        foreach (var item in items)
        {
            if (item.itemName == keyName && item.itemType == ItemType.Key)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Remove a key from the player's inventory
    /// </summary>
    public void RemoveKey(string keyName)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemName == keyName && items[i].itemType == ItemType.Key)
            {
                items.RemoveAt(i);
                OnInventoryChanged?.Invoke();
                Debug.Log($"Removed key: {keyName}");
                return;
            }
        }
        Debug.Log($"Key {keyName} not found in inventory");
    }

    /// <summary>
    /// Get all items in the inventory
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        return items;
    }
} 