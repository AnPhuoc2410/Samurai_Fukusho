using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Damage Pickup")]
    [Tooltip("Gắn script này vào prefab item tăng damage. Khi player nhặt sẽ tăng damage gây ra (theo % hoặc số lượng cố định), hỗ trợ buff tạm thời hoặc vĩnh viễn.")]
    public class DamagePickup : PickupBaseLogic
    {
        [Header("Damage Buff Settings")]
        [Tooltip("Nếu true, tăng damage theo phần trăm. Nếu false, tăng damage theo số lượng cố định.")]
        public bool increaseByPercent = true;
        [Tooltip("Giá trị tăng damage (phần trăm hoặc số lượng cố định)")]
        public float increaseValue = 20f;
        [Tooltip("SFX khi kết thúc buff damage (tùy chọn)")]
        public AudioClip buffEndSFX;
        
        [Header("Item Settings")]
        [Tooltip("Tên hiển thị của damage boost trong inventory.")]
        public string itemName = "Damage Boost";

        protected override void OnPickupEffect(Collider2D player)
        {
            // Áp dụng buff damage
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDamageBuff(increaseByPercent, increaseValue, 0f, null, buffEndSFX);
            }
            
            // Thêm vào inventory
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                // Lấy sprite từ SpriteRenderer
                Sprite itemSprite = GetComponent<SpriteRenderer>()?.sprite;
                
                if (itemSprite != null)
                {
                    inventory.AddItem(itemName, itemSprite, ItemType.Weapon);
                }
            }
        }
        
        protected override void OnBuffStart(Collider2D player)
        {
            // Áp dụng buff damage tạm thời
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDamageBuff(increaseByPercent, increaseValue, buffDuration, null, buffEndSFX);
            }
            
            // Thêm vào inventory
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                // Lấy sprite từ SpriteRenderer
                Sprite itemSprite = GetComponent<SpriteRenderer>()?.sprite;
                
                if (itemSprite != null)
                {
                    inventory.AddItem(itemName, itemSprite, ItemType.Weapon);
                }
            }
        }
        
        protected override void OnBuffEnd(GameObject playerObj)
        {
            var pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RemoveDamageBuff(buffEndSFX);
            }
        }
    }
} 