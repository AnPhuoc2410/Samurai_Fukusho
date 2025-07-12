using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    public UnityEvent NoColliderRemain;
    Collider2D col;
    public List<Collider2D> detectedColliders = new();

    private void Awake()
    {
        col = GetComponent<Collider2D>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
        if (detectedColliders.Count <= 0)
        {
            NoColliderRemain?.Invoke();
        }
    }
}
