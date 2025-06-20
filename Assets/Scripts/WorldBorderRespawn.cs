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
        if (startingPoint != null)
        {
        }
        
        bool hasRigidbody2D = GetComponent<Rigidbody2D>() != null;
        bool hasCollider2D = GetComponent<Collider2D>() != null;
        
        
        if (!hasRigidbody2D)
        {
        }
        
        if (!hasCollider2D)
        {
        }
        
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D != null)
        {
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleRespawn(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleRespawn(other.gameObject);
    }

    private void HandleRespawn(GameObject hitObject)
    {        
        if (hitObject.layer == gameGroundLayer)
        {
            if (startingPoint != null)
            {
                StartCoroutine(TeleportNextFrame());
            }
            else
            {
            }
        }
        else
        {
        }
    }

    private IEnumerator TeleportNextFrame()
    {        
        yield return null;
        
        
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {            
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            
            rb2D.position = startingPoint.position;            
        }
        else
        {
            transform.position = startingPoint.position;
        }
        
    }
}
