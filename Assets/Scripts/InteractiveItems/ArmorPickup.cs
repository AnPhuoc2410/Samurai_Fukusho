using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Armor Pickup")]
    [Tooltip("Gắn script này vào prefab item tăng giáp. Khi player nhặt sẽ giảm damage nhận vào (theo % hoặc số lượng cố định), hỗ trợ buff tạm thời hoặc vĩnh viễn.")]
    public class ArmorPickup : PickupBaseLogic
    {
        [Header("Armor Settings")]
        [Tooltip("Nếu true, giảm damage theo phần trăm. Nếu false, giảm damage theo số lượng cố định.")]
        public bool reduceByPercent = true;
        [Tooltip("Giá trị giảm damage (phần trăm hoặc số lượng cố định)")]
        public float reduceValue = 20f;
        [Tooltip("SFX khi bắt đầu buff giáp (tùy chọn)")]
        public AudioClip buffStartSFX;
        [Tooltip("SFX khi kết thúc buff giáp (tùy chọn)")]
        public AudioClip buffEndSFX;

        protected override void OnPickupEffect(Collider2D player)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDefenseBuff(reduceByPercent, reduceValue, 0f, buffStartSFX, buffEndSFX);
            }
        }
        protected override void OnBuffStart(Collider2D player)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyDefenseBuff(reduceByPercent, reduceValue, buffDuration, buffStartSFX, buffEndSFX);
            }
        }
        protected override void OnBuffEnd(GameObject playerObj)
        {
            var pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.RemoveDefenseBuff(buffEndSFX);
            }
        }
    }
} 