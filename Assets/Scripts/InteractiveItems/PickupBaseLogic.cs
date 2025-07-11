using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Pickup Base Logic")]
    [Tooltip("Base class cho mọi loại item pickup. Kế thừa class này để tạo các loại item như Health, Key, Powerup...")]
    public abstract class PickupBaseLogic : MonoBehaviour
    {
        [Header("Pickup SFX")]
        public AudioClip pickupSFX;
        public float sfxVolume = 1f;

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsPlayer(other))
            {
                OnPickupEffect(other);

                if (pickupSFX != null)
                    AudioSource.PlayClipAtPoint(pickupSFX, transform.position, sfxVolume);

                Destroy(gameObject);
            }
        }

        protected virtual bool IsPlayer(Collider2D other)
        {
            return other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Player");
        }

        // Logic riêng cho từng loại item
        protected abstract void OnPickupEffect(Collider2D player);
    }
} 