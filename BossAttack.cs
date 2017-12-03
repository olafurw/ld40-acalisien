using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    AudioSource myAudioSource = null;
    AudioClip myHitClip = null;

    float myChargeUpTime = 2.4f;
    float myChargeTimer = 0.0f;

    SpriteRenderer mySpriteRenderer;

    public bool myIsActive = false;

    void Start()
    {
        myIsActive = false;

        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color tempColor = mySpriteRenderer.color;
        tempColor.a = 0.0f;
        mySpriteRenderer.color = tempColor;

        myAudioSource = GetComponent<AudioSource>();
        myHitClip = Resources.Load<AudioClip>("boss-attack-hit");

        Invoke("AttackHit", myChargeUpTime);
    }

    void OnCollisionEnter2D(Collision2D aCollider)
    {
        
    }

    void OnTriggerEnter2D(Collider2D aCollider)
    {
        
    }

    void AttackHit()
    {
        myIsActive = true;

        myAudioSource.PlayOneShot(myHitClip);

        Invoke("AttackDone", 0.2f);
    }

    void AttackDone()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        myChargeTimer += Time.deltaTime;

        Color tempColor = mySpriteRenderer.color;
        tempColor.a = Mathf.Lerp(0.0f, 1.0f, myChargeTimer / myChargeUpTime);
        mySpriteRenderer.color = tempColor;
    }
}
