using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Health Pickup")]
    [Tooltip("Gắn script này vào prefab item hồi máu. Khi player nhặt sẽ tự động hồi máu cho player.")]
    public class HealthPickup : PickupBaseLogic
    {
        [Header("Health Settings")]
        [Tooltip("Lượng máu sẽ hồi cho player khi nhặt item này.")]
        public int healthAmount = 20;

        protected override void OnPickupEffect(Collider2D player)
        {
            Damageable damageable = player.GetComponent<Damageable>();
            if (damageable)
                damageable.Heal(healthAmount);
        }
    }
}
