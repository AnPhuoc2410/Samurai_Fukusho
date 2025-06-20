using UnityEngine;
using System.Collections;

public class WorldBorderRespawn : MonoBehaviour
{
    [Tooltip("Assign the starting point Transform here.")]
    public Transform startingPoint;

    [Tooltip("Layer to detect as GameGround (set to match GameGround layer number)")]
    public int gameGroundLayer = 6; // Change to your actual GameGround layer number

    private void Awake()
    {
        Debug.Log($"[WorldBorderRespawn] Awake called on {gameObject.name}");
    }

    private void Start()
    {     
        bool hasRigidbody2D = GetComponent<Rigidbody2D>() != null;
        bool hasCollider2D = GetComponent<Collider2D>() != null;       
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
        if (hitObject.layer == gameGroundLayer && startingPoint != null)
        {
            StartCoroutine(TeleportNextFrame());
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