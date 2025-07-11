using UnityEngine;
using System.Collections.Generic;

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

    [SerializeField] private List<string> keys = new List<string>();

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
    /// Add a key to the player's inventory
    /// </summary>
    /// <param name="keyName">Name of the key to add</param>
    public void AddKey(string keyName)
    {
        if (!keys.Contains(keyName))
        {
            keys.Add(keyName);
            Debug.Log($"Added key: {keyName}");
        }
        else
        {
            Debug.Log($"Key {keyName} already exists in inventory");
        }
    }

    /// <summary>
    /// Check if player has a specific key
    /// </summary>
    /// <param name="keyName">Name of the key to check</param>
    /// <returns>True if player has the key, false otherwise</returns>
    public bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

    /// <summary>
    /// Remove a key from the player's inventory (optional, for consumable keys)
    /// </summary>
    /// <param name="keyName">Name of the key to remove</param>
    public void RemoveKey(string keyName)
    {
        if (keys.Contains(keyName))
        {
            keys.Remove(keyName);
            Debug.Log($"Removed key: {keyName}");
        }
        else
        {
            Debug.Log($"Key {keyName} not found in inventory");
        }
    }

    /// <summary>
    /// Get all keys in the inventory
    /// </summary>
    /// <returns>List of all key names</returns>
    public List<string> GetAllKeys()
    {
        return new List<string>(keys);
    }

    /// <summary>
    /// Clear all keys from inventory
    /// </summary>
    public void ClearAllKeys()
    {
        keys.Clear();
        Debug.Log("All keys cleared from inventory");
    }
} 