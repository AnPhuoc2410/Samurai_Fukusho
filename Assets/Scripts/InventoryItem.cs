using UnityEngine;

[System.Serializable]
public struct InventoryItem
{
    public string itemName;
    public Sprite sprite;
    public ItemType itemType;

    public InventoryItem(string name, Sprite itemSprite)
    {
        itemName = name;
        sprite = itemSprite;
        itemType = ItemType.Key; // Default type
    }

    public InventoryItem(string name, Sprite itemSprite, ItemType type)
    {
        itemName = name;
        sprite = itemSprite;
        itemType = type;
    }
}

public enum ItemType
{
    Key,
    Potion,
    Weapon,
    Misc
} 