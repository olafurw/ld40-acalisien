using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Sprite mySprite;
    Sprite myHitSprite;
    Sprite myDeadSprite;
    SpriteRenderer mySpriteRenderer;

    GameObject myAttackPrefab;

    public bool myIsAlive = true;

    public float myHealth = 100.0f;
    bool myIsInHitState = false;
    float myHitStateLength = 0.025f;
    float myHitStateInitialLength = 0.025f;

    float myAggroDistance = 10.0f;
    bool myIsAggroed = false;

    GameObject myTarget;
    Player myPlayer;

    AudioClip myHitClip = null;
    AudioClip myDeadClip = null;
    AudioSource myAudioSource = null;

    void Start()
    {
        myIsAlive = true;
        myIsInHitState = false;
        myIsAggroed = false;

        myAttackPrefab = Resources.Load<GameObject>("Boss-Attack");

        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        mySprite = Resources.Load<Sprite>("boss");
        myHitSprite = Resources.Load<Sprite>("boss-hit");
        myDeadSprite = Resources.Load<Sprite>("boss-dead");

        myTarget = GameObject.FindGameObjectWithTag("Player");
        myPlayer = myTarget.GetComponent<Player>();

        myAudioSource = GetComponent<AudioSource>();
        myHitClip = Resources.Load<AudioClip>("enemy-hit");
        myDeadClip = Resources.Load<AudioClip>("enemy-dead");
    }

    void OnEnable()
    {
        EventManager.StartListening("Aggroed", OnAggroed);
    }

    void OnCollisionEnter2D(Collision2D aCollider)
    {
    }

    void OnTriggerEnter2D(Collider2D aCollider)
    {
        if (!myIsAlive)
        {
            Destroy(aCollider.gameObject);

            return;
        }

        if (!myIsAggroed)
        {
            return;
        }

        if (myIsAlive
            && aCollider.gameObject.name == "Bullet")
        {
            if (!myIsInHitState)
            {
                myHitStateLength = myHitStateInitialLength;
                mySpriteRenderer.sprite = myHitSprite;
            }

            myIsInHitState = true;

            Destroy(aCollider.gameObject);

            myHealth -= 1.0f;
            if (myHealth <= 0.0f)
            {
                myHealth = 0.0f;
                myIsAlive = false;
                mySpriteRenderer.sprite = myDeadSprite;

                myAudioSource.PlayOneShot(myDeadClip);

                EventManager.TriggerEvent("BossDied");
            }
            else
            {
                if (!myAudioSource.isPlaying)
                {
                    myAudioSource.PlayOneShot(myHitClip, 0.8f);
                }
            }
        }
    }

    void OnAggroed()
    {
        Invoke("Attack", Random.Range(4.0f, 6.0f));
    }

    void Attack()
    {
        if (!myIsAlive)
        {
            return;
        }

        GameObject newBullet = Instantiate(myAttackPrefab, myPlayer.transform.position, transform.rotation) as GameObject;
        newBullet.name = "Boss-Attack";

        Invoke("Attack", Random.Range(4.0f, 6.0f));
    }

    void FixedUpdate()
    {
    }

    void Update()
    {
        if (!myIsAlive)
        {
            return;
        }

        if (myIsInHitState)
        {
            myHitStateLength -= Time.deltaTime;

            if (myHitStateLength <= 0.0f)
            {
                myIsInHitState = false;

                mySpriteRenderer.sprite = mySprite;
            }
        }

        PrivRotateAndMoveToTarget();
    }

    // https://answers.unity.com/answers/651344/view.html
    void PrivRotateAndMoveToTarget()
    {
        if (!myPlayer.myIsAlive)
        {
            return;
        }

        float distance = Vector2.Distance(
            new Vector2(myPlayer.transform.position.x, myPlayer.transform.position.y),
            new Vector2(transform.position.x, transform.position.y)
        );

        if (!myIsAggroed
            && distance > myAggroDistance)
        {
            return;
        }

        if (!myIsAggroed)
        {
            EventManager.TriggerEvent("Aggroed");
        }

        myIsAggroed = true;

        transform.position = Vector2.MoveTowards(transform.position, myTarget.transform.position, 0.5f * Time.deltaTime);
    }
}
