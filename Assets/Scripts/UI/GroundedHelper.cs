using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundedHelper : MonoBehaviour
{

    [SerializeField] List<Collider> colliders;
    public bool isGrounded;

    void Awake()
    {
        colliders = new List<Collider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) return;
        
        colliders.Add(other);
        isGrounded = true;

    }
    void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
        if(colliders.Count == 0){isGrounded = false;}
    }
}
