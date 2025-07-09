using UnityEngine;
using System.Linq;

public class PressE_ToOpen : MonoBehaviour
{
    public float interactRange = 2f;
    private GameObject player;
    private Animator animator;
    private bool isOpened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Thử tìm Player bằng tag trước
        player = GameObject.FindGameObjectWithTag("Player");

        // Nếu không tìm thấy bằng tag, thử tìm bằng layer
        if (player == null)
        {
            GameObject[] objectsInPlayerLayer = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None)
                .Where(go => go.layer == LayerMask.NameToLayer("Player"))
                .ToArray();

            if (objectsInPlayerLayer.Length > 0)
            {
                player = objectsInPlayerLayer[0];
                if (objectsInPlayerLayer.Length > 1)
                {
                    Debug.LogWarning($"[{gameObject.name}] Found multiple objects in Player layer. Using the first one found.");
                }
            }
        }

        // Nếu vẫn không tìm thấy, hiện thông báo lỗi
        if (player == null)
        {
            Debug.LogError($"[{gameObject.name}] Player instance not found! Please ensure Player has either 'Player' tag or is in 'Player' layer.");
        }
    }

    void Update()
    {
        if (player == null || isOpened) return;

        float distance = Vector2.Distance(player.transform.position, transform.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("isOpen", true);
            isOpened = true;

            // TODO: Add mở loot, âm thanh, hiệu ứng, v.v.
        }
    }
}
