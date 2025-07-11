using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject itemDisplayPrefab;
    public Transform itemContainer;

    private List<GameObject> displayedItems = new List<GameObject>();

    private void Start()
    {
        // Subscribe to inventory changes
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged.AddListener(RefreshDisplay);
        }
    }

    private void RefreshDisplay()
    {
        // Clear current display
        foreach (GameObject item in displayedItems)
        {
            if (item != null)
                Destroy(item);
        }
        displayedItems.Clear();

        // Show all items
        var items = PlayerInventory.Instance.GetAllItems();
        foreach (var item in items)
        {
            if (item.sprite != null)
            {
                GameObject newItemDisplay = Instantiate(itemDisplayPrefab, itemContainer);
                Image icon = newItemDisplay.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = item.sprite;
                }
                displayedItems.Add(newItemDisplay);
            }
        }
    }
} 