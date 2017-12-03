using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 myTarget;
    public float myBulletSpeed = 10.0f;
    public float myLifetimeSec = 1.0f;

    void Start()
    {
        myTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PrivUpdateRotation();
    }

    void OnCollisionEnter2D(Collision2D aCollider)
    {
    }

    void OnTriggerEnter2D(Collider2D aCollider)
    {
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        PrivMoveForwards();
        Destroy(gameObject, myLifetimeSec);
    }

    private void PrivMoveForwards()
    {
        transform.Translate(transform.up * myBulletSpeed * Time.deltaTime, Space.World);
    }

    // https://answers.unity.com/answers/637243/view.html
    private void PrivUpdateRotation()
    {
        Vector3 difference = myTarget - transform.position;
        difference.Normalize();

        float zRotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        zRotation += Random.Range(-10.0f, 10.0f);

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, zRotation - 90.0f);
    }
}
