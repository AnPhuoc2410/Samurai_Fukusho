using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Speed Pickup")]
    [Tooltip("Gắn script này vào prefab item tăng tốc độ. Khi player nhặt sẽ tăng tốc độ di chuyển (theo % hoặc số lượng cố định), hỗ trợ buff tạm thời hoặc vĩnh viễn.")]
    public class SpeedPickup : PickupBaseLogic
    {
        [Header("Speed Settings")]
        [Tooltip("Nếu true, tăng tốc độ theo phần trăm. Nếu false, tăng tốc độ theo số lượng cố định.")]
        public bool increaseByPercent = true;
        [Tooltip("Giá trị tăng tốc độ (phần trăm hoặc số lượng cố định)")]
        public float increaseValue = 20f;
        [Tooltip("SFX khi kết thúc buff tốc độ (tùy chọn)")]
        public AudioClip buffEndSFX;
        
        [Header("Item Settings")]
        [Tooltip("Tên hiển thị của speed boost trong inventory.")]
        public string itemName = "Speed Boost";

        protected override void OnPickupEffect(Collider2D player)
        {
            // Áp dụng buff tốc độ
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplySpeedBuff(increaseByPercent, increaseValue, 0f, null, buffEndSFX);
            }
            
            // Thêm vào inventory
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                // Lấy sprite từ SpriteRenderer
                Sprite itemSprite = GetComponent<SpriteRenderer>()?.sprite;
                
                if (itemSprite != null)
                {
                    inventory.AddItem(itemName, itemSprite, ItemType.Misc);
                }
            }
        }
        
        protected override void OnBuffStart(Collider2D player)
        {
            // Áp dụng buff tốc độ tạm thời
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplySpeedBuff(increaseByPercent, increaseValue, buffDuration, null, buffEndSFX);
            }
            
            // Thêm vào inventory
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                // Lấy sprite từ SpriteRenderer
                Sprite itemSprite = GetComponent<SpriteRenderer>()?.sprite;
                
                if (itemSprite != null)
                {
                    inventory.AddItem(itemName, itemSprite, ItemType.Misc);
                }
            }
        }
        
        protected override void OnBuffEnd(GameObject playerObj)
        {
            var pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RemoveSpeedBuff(buffEndSFX);
            }
        }
    }
} 