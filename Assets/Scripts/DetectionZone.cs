using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
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
    }
}
