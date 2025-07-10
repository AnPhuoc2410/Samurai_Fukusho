using UnityEngine;
using System.Linq;

public abstract class PressE_ToOpen : MonoBehaviour
{
    public float interactRange = 2f;
    protected GameObject player;
    protected Animator animator;
    protected bool isOpened = false;
    protected Collider2D interactableCollider;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        interactableCollider = GetComponent<Collider2D>();
        
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

        // Kiểm tra nếu không có Collider2D
        if (interactableCollider == null)
        {
            Debug.LogError($"[{gameObject.name}] No Collider2D found! Please add a Collider2D component.");
        }
    }

    protected virtual void Update()
    {
        if (player == null || isOpened) return;

        float distance = Vector2.Distance(player.transform.position, transform.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }

    // Phương thức cơ bản khi tương tác, có thể override trong các class con
    protected virtual void OnInteract()
    {
        animator.SetBool("isOpen", true);
        isOpened = true;

        // Tắt collider khi mở
        if (interactableCollider != null)
        {
            interactableCollider.enabled = false;
        }
    }

    // Helper method để kiểm tra xem có thể tương tác không
    public bool CanInteract()
    {
        if (player == null || isOpened) return false;
        float distance = Vector2.Distance(player.transform.position, transform.position);
        return distance <= interactRange;
    }
}
