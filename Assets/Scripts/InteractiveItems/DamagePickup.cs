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
        [Tooltip("SFX khi bắt đầu buff damage (tùy chọn)")]
        public AudioClip buffStartSFX;
        [Tooltip("SFX khi kết thúc buff damage (tùy chọn)")]
        public AudioClip buffEndSFX;

        protected override void OnPickupEffect(Collider2D player)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDamageBuff(increaseByPercent, increaseValue, 0f, buffStartSFX, buffEndSFX);
            }
        }
        protected override void OnBuffStart(Collider2D player)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDamageBuff(increaseByPercent, increaseValue, buffDuration, buffStartSFX, buffEndSFX);
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