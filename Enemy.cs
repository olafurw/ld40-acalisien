using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Sprite mySprite;
    Sprite myHitSprite;
    Sprite myDeadSprite;
    SpriteRenderer mySpriteRenderer;

    public bool myIsAlive = true;

    public float myHealth = 10.0f;
    bool myIsInHitState = false;
    float myHitStateLength = 0.025f;
    float myHitStateInitialLength = 0.025f;

    float myAggroDistance = 5.0f;
    bool myIsAggroed = false;

    Game myGame;
    bool myCanMove = false;

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

        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        mySprite = Resources.Load<Sprite>("enemy");
        myHitSprite = Resources.Load<Sprite>("enemy-hit");
        myDeadSprite = Resources.Load<Sprite>("enemy-dead");

        myTarget = GameObject.FindGameObjectWithTag("Player");
        myPlayer = myTarget.GetComponent<Player>();

        myAudioSource = GetComponent<AudioSource>();
        myHitClip = Resources.Load<AudioClip>("enemy-hit");
        myDeadClip = Resources.Load<AudioClip>("enemy-dead");

        myGame = GameObject.FindGameObjectWithTag("Game").GetComponent<Game>();
    }

    void OnEnable()
    {
        EventManager.StartListening("GameStateChanged", OnGameStateChanged);
    }

    void OnDisable()
    {
        EventManager.StopListening("GameStateChanged", OnGameStateChanged);
    }

    void OnGameStateChanged()
    {
        if (myGame.myState == Game.GameState.Move)
        {
            myCanMove = true;
        }
        else if (myGame.myState == Game.GameState.Stop)
        {
            myCanMove = false;
        }
        else if (myGame.myState == Game.GameState.End)
        {
            myCanMove = false;
        }
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

    void FixedUpdate()
    {
    }

    void Update()
    {
        if (!myIsAlive
            || !myCanMove)
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

        myIsAggroed = true;

        Vector3 vectorToTarget = myTarget.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5.0f);

        transform.position = Vector2.MoveTowards(transform.position, myTarget.transform.position, 1.0f * Time.deltaTime);
    }
}
