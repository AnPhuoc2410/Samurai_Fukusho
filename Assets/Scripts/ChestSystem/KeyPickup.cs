using UnityEngine;
using InteractiveItems;

[AddComponentMenu("Items/Key Pickup")]
[Tooltip("Gắn script này vào prefab chìa khóa. Khi player nhặt sẽ tự động add key vào PlayerInventory.")]
public class KeyPickup : PickupBaseLogic
{
    [Header("Key Settings")]
    [Tooltip("Tên định danh của chìa khóa sẽ được add vào PlayerInventory. Nếu chỉ có 1 loại chìa khóa, giữ nguyên mặc định.")]
    [SerializeField] private string keyName = "Level1Key";

    protected override void OnPickupEffect(Collider2D player)
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory != null)
        {
            inventory.AddKey(keyName);
        }
        else
        {
            Debug.LogError("PlayerInventory instance not found! Make sure PlayerInventory is attached to a GameObject in the scene.");
        }
    }
}
// Để có hiệu ứng spin/floating, hãy gán thêm script PickupVisualEffect vào prefab item này. 