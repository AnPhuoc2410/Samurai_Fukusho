using UnityEngine;
using System.Collections.Generic;

namespace ChestSystem
{
    public class InteractableChest : PressE_ToOpen
    {
        [Header("Spawn Settings")]
        [Tooltip("Bán kính spawn item quanh rương.")]
        public float spawnRadius = 1.2f;
        [Tooltip("Số lần thử tránh overlap khi spawn item.")]
        public int maxSpawnTries = 10;
        [Tooltip("LayerMask dùng để kiểm tra va chạm khi spawn item.")]
        public LayerMask itemLayerMask;

        [Header("Chest Items")]
        [Tooltip("Các prefab item sẽ spawn khi mở rương. Sẽ được random và gán tự động bởi ChestKeyRandomizer.")]
        public List<GameObject> itemPrefabs = new List<GameObject>(5);

        [Header("Override Item Count (Optional)")]
        [Tooltip("Bật để override số lượng item min/max cho rương này, thay vì dùng giá trị global.")]
        public bool overrideItemCount = false;
        [Tooltip("Số lượng item tối thiểu cho rương này (chỉ dùng khi overrideItemCount = true)")]
        public int minItemsOverride = 1;
        [Tooltip("Số lượng item tối đa cho rương này (chỉ dùng khi overrideItemCount = true)")]
        public int maxItemsOverride = 5;

        [Header("Auto Destroy (Set by Randomizer)")]
        [Tooltip("Bật nếu item spawn ra từ rương này sẽ tự hủy sau một thời gian (set tự động bởi ChestKeyRandomizer)")]
        public bool enableAutoDestroy = false;
        [Tooltip("Thời gian (giây) trước khi item tự hủy (set tự động bởi ChestKeyRandomizer)")]
        public float autoDestroyTime = 20f;

        private bool hasKey = false;

        protected override void OnInteract()
        {
            base.OnInteract();
            SpawnAllItems();
        }

        public void AddKeyToChest(GameObject keyPrefabToAdd)
        {
            if (itemPrefabs.Count < 5)
            {
                itemPrefabs.Add(keyPrefabToAdd);
            }
            else
            {
                int idx = Random.Range(0, itemPrefabs.Count);
                itemPrefabs[idx] = keyPrefabToAdd;
            }
            hasKey = true;
        }

        private void SpawnAllItems()
        {
            Vector2 chestPos = transform.position;
            List<Vector2> usedPositions = new List<Vector2>();

            for (int i = 0; i < itemPrefabs.Count; i++)
            {
                GameObject prefab = itemPrefabs[i];
                Vector2 spawnPos = chestPos;
                bool found = false;
                int tries = 0;
                while (!found && tries < maxSpawnTries)
                {
                    float xOffset = Random.Range(-spawnRadius, spawnRadius);
                    float yOffset = Random.Range(spawnRadius * 0.5f, spawnRadius); // luôn dương, chỉ phía trên
                    Vector2 candidate = chestPos + new Vector2(xOffset, yOffset);
                    bool overlap = false;
                    foreach (var pos in usedPositions)
                    {
                        if (Vector2.Distance(candidate, pos) < 0.5f) { overlap = true; break; }
                    }
                    if (!Physics2D.OverlapCircle(candidate, 0.3f, itemLayerMask) && !overlap)
                    {
                        spawnPos = candidate;
                        found = true;
                    }
                    tries++;
                }
                usedPositions.Add(spawnPos);
                var go = Instantiate(prefab, spawnPos, Quaternion.identity);
                if (enableAutoDestroy)
                {
                    var autoDestroy = go.AddComponent<ItemAutoDestroy>();
                    autoDestroy.lifetime = autoDestroyTime;
                }
            }
        }
    }

    [AddComponentMenu("Chest System/Item Auto Destroy")]
    [Tooltip("Gắn script này vào item để tự hủy sau một thời gian. Sẽ được add tự động khi spawn nếu enableAutoDestroy.")]
    public class ItemAutoDestroy : MonoBehaviour
    {
        [Tooltip("Thời gian (giây) trước khi item tự hủy.")]
        public float lifetime = 20f;
        private void Start() { Destroy(gameObject, lifetime); }
    }
}
