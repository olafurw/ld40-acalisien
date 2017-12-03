using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject myPlayer = null;
    
    public float myPlayerSpeed = 1.0f;

    MachineGun myMachineGun;
    ShotGun myShotGun;

    AudioClip myPlayerDeath = null;
    AudioSource myAudioSource = null;

    Game myGame;
    bool myCanMove = false;

    public bool myIsAlive = true;

    bool myIsUsingMachineGun = true;

    void Start()
    {
        myIsAlive = true;
        myCanMove = false;

        myAudioSource = GetComponent<AudioSource>();
        myPlayerDeath = Resources.Load<AudioClip>("player-death");

        myPlayer = GameObject.FindGameObjectWithTag("Player");
        myMachineGun = GetComponent<MachineGun>();
        myShotGun = GetComponent<ShotGun>();
        myGame = GameObject.FindGameObjectWithTag("Game").GetComponent<Game>();
    }

    void FixedUpdate()
    {
    }

    void OnEnable()
    {
        EventManager.StartListening("GameStateChanged", OnGameStateChanged);
    }

    void OnDisable()
    {
        EventManager.StopListening("GameStateChanged", OnGameStateChanged);
    }

    void OnCollisionEnter2D(Collision2D aCollider)
    {
        if (!myIsAlive)
        {
            return;
        }

        if(aCollider.gameObject.name == "Enemy"
           || aCollider.gameObject.name == "Boss")
        {
            bool isAlive = false;
            if (aCollider.gameObject.name == "Enemy")
            {
                Enemy enemy = aCollider.gameObject.GetComponent<Enemy>();
                isAlive = enemy.myIsAlive;
            }
            if (aCollider.gameObject.name == "Boss")
            {
                Boss enemy = aCollider.gameObject.GetComponent<Boss>();
                isAlive = enemy.myIsAlive;
            }

            if (isAlive)
            {
                myIsAlive = false;
                EventManager.TriggerEvent("PlayerDied");

                myAudioSource.PlayOneShot(myPlayerDeath);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D aCollider)
    {
        if (!myIsAlive)
        {
            return;
        }

        if (aCollider.gameObject.name == "shotgun-pickup")
        {
            myIsUsingMachineGun = false;
            Destroy(aCollider.gameObject);

            return;
        }

        if (aCollider.gameObject.name == "Boss-Attack")
        {
            BossAttack attack = aCollider.gameObject.GetComponent<BossAttack>();

            if (attack.myIsActive)
            {
                myIsAlive = false;
                EventManager.TriggerEvent("PlayerDied");

                myAudioSource.PlayOneShot(myPlayerDeath);
            }

            return;
        }
    }

    void OnTriggerStay2D(Collider2D aCollider)
    {
        if (!myIsAlive)
        {
            return;
        }

        if (aCollider.gameObject.name == "Boss-Attack")
        {
            BossAttack attack = aCollider.gameObject.GetComponent<BossAttack>();

            if (attack.myIsActive)
            {
                myIsAlive = false;
                EventManager.TriggerEvent("PlayerDied");

                myAudioSource.PlayOneShot(myPlayerDeath);
            }

            return;
        }
    }

    void OnGameStateChanged()
    {
        if(myGame.myState == Game.GameState.Move)
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

    void Update()
    {
        if (!myCanMove
            || !myIsAlive)
        {
            return;
        }

        PrivUpdatePlayerRotation();
        PrivMovePlayer();

        bool isButtonHeld = Input.GetMouseButton(0);
        bool isButtonJustPressed = Input.GetMouseButtonDown(0);

        if (isButtonJustPressed || isButtonHeld)
        {
            if (myIsUsingMachineGun)
            {
                myMachineGun.Shoot(myPlayer.transform);
            }
            else
            {
                myShotGun.Shoot(myPlayer.transform);
            }
        }
    }

    // https://answers.unity.com/answers/667654/view.html
    private void PrivMovePlayer()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        myPlayer.transform.position = new Vector3(
            Mathf.Lerp(myPlayer.transform.position.x, movement.x + myPlayer.transform.position.x, myPlayerSpeed * Time.deltaTime),
            Mathf.Lerp(myPlayer.transform.position.y, movement.y + myPlayer.transform.position.y, myPlayerSpeed * Time.deltaTime),
            0.0f
        );
    }

    // https://answers.unity.com/answers/637243/view.html
    private void PrivUpdatePlayerRotation()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mousePoint - myPlayer.transform.position;
        difference.Normalize();

        float zRotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        myPlayer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, zRotation - 90.0f);
    }
}
