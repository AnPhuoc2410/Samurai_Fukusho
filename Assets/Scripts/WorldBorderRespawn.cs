using UnityEngine;
using System.Collections;

public class WorldBorderRespawn : MonoBehaviour
{
    [Tooltip("Assign the starting point Transform here.")]
    public Transform startingPoint;

    // Set this to the layer number of 'GameGround' in the Inspector or via code
    [Tooltip("Layer to detect as GameGround (set to match GameGround layer number)")]
    public int gameGroundLayer = 6; // Change 8 to your actual GameGround layer number

    private void Awake()
    {
        Debug.Log($"[WorldBorderRespawn] Awake called on {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"[WorldBorderRespawn] Start called on {gameObject.name}");
        Debug.Log($"[WorldBorderRespawn] Looking for layer: {gameGroundLayer}");
        Debug.Log($"[WorldBorderRespawn] Starting point assigned: {startingPoint != null}");
        if (startingPoint != null)
        {
            Debug.Log($"[WorldBorderRespawn] Starting point position: {startingPoint.position}");
        }
        
        bool hasRigidbody2D = GetComponent<Rigidbody2D>() != null;
        bool hasCollider2D = GetComponent<Collider2D>() != null;
        
        Debug.Log($"[WorldBorderRespawn] Has Rigidbody2D: {hasRigidbody2D}");
        Debug.Log($"[WorldBorderRespawn] Has Collider2D: {hasCollider2D}");
        
        if (!hasRigidbody2D)
        {
            Debug.LogError("[WorldBorderRespawn] MISSING RIGIDBODY2D! Add a Rigidbody2D component to this GameObject for 2D collision detection to work!");
        }
        
        if (!hasCollider2D)
        {
            Debug.LogError("[WorldBorderRespawn] MISSING COLLIDER2D! Add a Collider2D component to this GameObject for 2D collision detection to work!");
        }
        
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D != null)
        {
            Debug.Log($"[WorldBorderRespawn] Collider2D isTrigger: {col2D.isTrigger}");
        }
        
        Debug.Log($"[WorldBorderRespawn] Script enabled: {enabled}");
        Debug.Log($"[WorldBorderRespawn] GameObject active: {gameObject.activeInHierarchy}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[WorldBorderRespawn] OnCollisionEnter2D called with {collision.gameObject.name}");
        HandleRespawn(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[WorldBorderRespawn] OnTriggerEnter2D called with {other.gameObject.name}");
        HandleRespawn(other.gameObject);
    }

    private void HandleRespawn(GameObject hitObject)
    {
        Debug.Log($"[WorldBorderRespawn] HandleRespawn called");
        Debug.Log($"[WorldBorderRespawn] Hit object: {hitObject.name}");
        Debug.Log($"[WorldBorderRespawn] Hit object layer: {hitObject.layer}");
        Debug.Log($"[WorldBorderRespawn] Target layer: {gameGroundLayer}");
        Debug.Log($"[WorldBorderRespawn] Layer match: {hitObject.layer == gameGroundLayer}");
        
        if (hitObject.layer == gameGroundLayer)
        {
            Debug.Log($"[WorldBorderRespawn] GameGround layer detected! Starting respawn process...");
            if (startingPoint != null)
            {
                Debug.Log($"[WorldBorderRespawn] Starting point is valid, starting coroutine...");
                StartCoroutine(TeleportNextFrame());
            }
            else
            {
                Debug.LogWarning("[WorldBorderRespawn] Starting point not assigned!");
            }
        }
        else
        {
            Debug.Log($"[WorldBorderRespawn] Layer mismatch, no respawn triggered");
        }
    }

    private IEnumerator TeleportNextFrame()
    {
        Debug.Log($"[WorldBorderRespawn] TeleportNextFrame coroutine started");
        Debug.Log($"[WorldBorderRespawn] Current position before teleport: {transform.position}");
        
        yield return null;
        
        Debug.Log($"[WorldBorderRespawn] Frame waited, proceeding with teleport...");
        
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            Debug.Log($"[WorldBorderRespawn] Rigidbody2D found, resetting velocities...");
            Debug.Log($"[WorldBorderRespawn] Current velocity: {rb2D.linearVelocity}");
            Debug.Log($"[WorldBorderRespawn] Current angular velocity: {rb2D.angularVelocity}");
            
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            
            Debug.Log($"[WorldBorderRespawn] Setting Rigidbody2D position to: {startingPoint.position}");
            rb2D.position = startingPoint.position;
            
            Debug.Log($"[WorldBorderRespawn] Rigidbody2D teleport complete");
            Debug.Log($"[WorldBorderRespawn] New Rigidbody2D position: {rb2D.position}");
        }
        else
        {
            Debug.Log($"[WorldBorderRespawn] No Rigidbody2D found, using Transform...");
            Debug.Log($"[WorldBorderRespawn] Setting Transform position to: {startingPoint.position}");
            transform.position = startingPoint.position;
            Debug.Log($"[WorldBorderRespawn] Transform teleport complete");
        }
        
        Debug.Log($"[WorldBorderRespawn] Final position after teleport: {transform.position}");
    }
}
