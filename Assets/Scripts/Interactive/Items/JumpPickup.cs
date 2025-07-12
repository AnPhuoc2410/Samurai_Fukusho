using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Jump Pickup")]
    [Tooltip("Gắn script này vào prefab item tăng sức mạnh nhảy. Khi player nhặt sẽ tăng lực nhảy (theo % hoặc số lượng cố định), hỗ trợ buff tạm thời hoặc vĩnh viễn.")]
    public class JumpPickup : PickupBaseLogic
    {
        [Header("Jump Buff Settings")]
        [Tooltip("Nếu true, tăng lực nhảy theo phần trăm. Nếu false, tăng lực nhảy theo số lượng cố định.")]
        public bool increaseByPercent = true;
        [Tooltip("Giá trị tăng lực nhảy (phần trăm hoặc số lượng cố định)")]
        public float increaseValue = 20f;
        [Tooltip("SFX khi kết thúc buff nhảy (tùy chọn)")]
        public AudioClip buffEndSFX;
        
        [Header("Item Settings")]
        [Tooltip("Tên hiển thị của jump boost trong inventory.")]
        public string itemName = "Jump Boost";

        protected override void OnPickupEffect(Collider2D player)
        {
            // Áp dụng buff nhảy
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyJumpBuff(increaseByPercent, increaseValue, 0f, null, buffEndSFX);
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
            // Áp dụng buff nhảy tạm thời
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyJumpBuff(increaseByPercent, increaseValue, buffDuration, null, buffEndSFX);
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
                pc.RemoveJumpBuff(buffEndSFX);
            }
        }
    }
} 