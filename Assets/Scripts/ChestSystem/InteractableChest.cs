using UnityEngine;
using System.Collections.Generic;

namespace ChestSystem
{
    public class InteractableChest : PressE_ToOpen
    {
        [Header("Chest Items")]
        [Tooltip("Các prefab item thường sẽ spawn khi mở rương (tối đa 5)")]
        public List<GameObject> itemPrefabs = new List<GameObject>(5);

        [Header("Key Chest Option")]
        [Tooltip("Chọn chế độ spawn chìa khóa: None = không, Random = random khi scene load, Manual = chỉ định rương này chứa chìa khóa")]
        public KeyChestMode keyChestMode = KeyChestMode.Random;
        public GameObject keyPrefab; // Prefab chìa khóa (nếu muốn chỉ định riêng)
        [Tooltip("Chỉ tick nếu muốn rương này chắc chắn chứa chìa khóa (chỉ dùng khi chọn Manual)")]
        public bool isManualKeyChest = false;

        [Header("Spawn Settings")]
        public float spawnRadius = 1.2f;
        public int maxSpawnTries = 10;
        public LayerMask itemLayerMask;

        private bool hasKey = false;

        public enum KeyChestMode { None, Random, Manual }

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
                // Nếu đã đủ 5 item, random 1 vị trí để thay thế
                int idx = Random.Range(0, itemPrefabs.Count);
                itemPrefabs[idx] = keyPrefabToAdd;
            }
            hasKey = true;
        }

        private void SpawnAllItems()
        {
            Vector2 chestPos = transform.position;
            float angleStep = 360f / Mathf.Max(1, itemPrefabs.Count);
            float angleOffset = Random.Range(0f, 360f);
            List<Vector2> usedPositions = new List<Vector2>();

            for (int i = 0; i < itemPrefabs.Count; i++)
            {
                GameObject prefab = itemPrefabs[i];
                Vector2 spawnPos = chestPos;
                bool found = false;
                int tries = 0;
                while (!found && tries < maxSpawnTries)
                {
                    float angle = angleOffset + angleStep * i + Random.Range(-10f, 10f);
                    Vector2 offset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * Random.Range(spawnRadius * 0.7f, spawnRadius);
                    Vector2 candidate = chestPos + offset;
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
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
