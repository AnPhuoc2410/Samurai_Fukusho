using UnityEngine;

namespace InteractiveItems
{
    [AddComponentMenu("Items/Pickup Visual Effect")]
    [Tooltip("Gắn script này vào prefab item để tạo hiệu ứng quay (spin) và lơ lửng (floating) cho item.")]
    public class PickupVisualEffect : MonoBehaviour
    {
        [Header("Spin Settings")]
        [Tooltip("Bật/tắt hiệu ứng quay cho item.")]
        public bool enableSpin = true;
        [Tooltip("Tốc độ/quỹ đạo quay của item (theo từng trục XYZ).")]
        public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

        [Header("Floating Settings")]
        [Tooltip("Bật/tắt hiệu ứng lơ lửng cho item.")]
        public bool enableFloating = false;
        [Tooltip("Tốc độ lơ lửng lên/xuống.")]
        public float floatSpeed = 2f;
        [Tooltip("Biên độ lơ lửng (độ cao tối đa lên/xuống so với vị trí gốc).")]
        public float floatAmplitude = 0.5f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            if (enableSpin)
            {
                transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
            }
            if (enableFloating)
            {
                Vector3 pos = startPos;
                pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
                transform.position = pos;
            }
        }
    }
} 