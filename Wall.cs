using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    void Start()
    {

    }

    void OnCollisionEnter2D(Collision2D aCollider)
    {
        if (aCollider.gameObject.name == "Bullet")
        {
            Destroy(aCollider.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D aCollider)
    {
        if (aCollider.gameObject.name == "Bullet")
        {
            Destroy(aCollider.gameObject);
        }
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        
    }
}
