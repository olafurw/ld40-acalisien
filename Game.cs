using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    GameObject myEnemyPrefab;

    Camera myCamera;
    GameObject myPlayer;

    Color myStartColor;
    Color myEndColor;
    Vector2 myEndPosition = new Vector2(63.0f, 33.0f);

    float myCameraIntroPanStart = 0.5f;
    float myCameraIntroPanEnd = 4.0f;
    float myCameraIntroPanSpeed = 0.15f;

    public enum GameState { Stop, Move, End };
    public GameState myState = GameState.Stop;

    bool myDeathCamera = false;

    AudioSource myAudioSource = null;
    AudioClip[] myTalkClips;
    UnityEngine.UI.Text myStoryText;

    int myIntroStoryState = 0;
    string[] myStoryTextString = new string[] { "Welcome child.", "You have been created.", "By me.", "You are a mistake.", "I will correct this mistake." };

    int myEndStoryState = 0;
    string[] myEndStoryTextString = new string[] { "How?", "I had everything", "I could create anything", "I was the mistake", "...", "I was the mistake" };

    void Start()
    {
        myState = GameState.Stop;

        myStartColor = new Color(229.0f / 255.0f, 145.0f / 255.0f, 145.0f / 255.0f);
        myEndColor = new Color(0.0f, 0.0f, 0.0f);

        myPlayer = GameObject.FindGameObjectWithTag("Player");
        myEnemyPrefab = Resources.Load<GameObject>("Enemy");

        myTalkClips = Resources.LoadAll<AudioClip>("talking");
        
        GameObject storyTextGo = GameObject.FindGameObjectWithTag("StoryText");
        myAudioSource = storyTextGo.GetComponent<AudioSource>();
        myStoryText = storyTextGo.GetComponent<UnityEngine.UI.Text>();

        GameObject cameraGo = GameObject.FindGameObjectWithTag("MainCamera") as GameObject;
        myCamera = cameraGo.GetComponent<Camera>();
        myCamera.orthographicSize = myCameraIntroPanStart;
        myCamera.backgroundColor = myStartColor;

        Invoke("SetNewStoryText", 1.0f);
    }

    void OnEnable()
    {
        EventManager.StartListening("GameStateChanged", OnGameStateChanged);
        EventManager.StartListening("PlayerDied", OnPlayerDied);
        EventManager.StartListening("BossDied", OnBossDied);
    }

    void OnDisable()
    {
        EventManager.StopListening("GameStateChanged", OnGameStateChanged);
        EventManager.StopListening("PlayerDied", OnPlayerDied);
        EventManager.StopListening("BossDied", OnBossDied);
    }

    void OnPlayerDied()
    {
        myDeathCamera = true;
    }

    void OnBossDied()
    {
        myState = GameState.End;
        EventManager.TriggerEvent("GameStateChanged");

        Invoke("SetEndStoryText", 1.0f);
    }

    void OnGameStateChanged()
    {

    }

    void Update()
    {
        float distanceToEnd = Vector2.Distance(
            new Vector2(myPlayer.transform.position.x, myPlayer.transform.position.y),
            myEndPosition
        );

        if (distanceToEnd > 75.0f)
        {
            distanceToEnd = 75.0f;
        }

        myCamera.backgroundColor = Color.Lerp(myEndColor, myStartColor, distanceToEnd / 75.0f);

        if (myDeathCamera)
        {
            if (myCamera.orthographicSize <= 0.0f)
            {
                myCamera.orthographicSize = 0.0f;

                return;
            }

            myCamera.orthographicSize -= 0.5f * Time.deltaTime;

            return;
        }

        myCamera.transform.position = new Vector3(
            Mathf.Lerp(myCamera.transform.position.x, myPlayer.transform.position.x, Time.deltaTime * 5.0f), 
            Mathf.Lerp(myCamera.transform.position.y, myPlayer.transform.position.y, Time.deltaTime * 5.0f), 
            -18
        );

        if (myState == GameState.Stop)
        {
            myCamera.orthographicSize += myCameraIntroPanSpeed * Time.deltaTime;
            if (myCamera.orthographicSize >= myCameraIntroPanEnd)
            {
                myCamera.orthographicSize = myCameraIntroPanEnd;

                myState = GameState.Move;
                EventManager.TriggerEvent("GameStateChanged");
            }
        }
    }

    void SetNewStoryText()
    {
        if (myIntroStoryState > myStoryTextString.Length - 1)
        {
            return;
        }

        myStoryText.CrossFadeAlpha(1.0f, 0.1f, false);
        myStoryText.text = myStoryTextString[myIntroStoryState];
        myAudioSource.PlayOneShot(myTalkClips[myIntroStoryState]);

        Invoke("FadeOutStoryText", 2.0f);
    }

    void FadeOutStoryText()
    {
        myStoryText.CrossFadeAlpha(0.0f, 1.0f, false);
        myIntroStoryState++;

        Invoke("SetNewStoryText", 2.0f);
    }

    void SetEndStoryText()
    {
        if (myEndStoryState > myEndStoryTextString.Length - 1)
        {
            return;
        }

        myStoryText.CrossFadeAlpha(1.0f, 0.1f, false);
        myStoryText.text = myEndStoryTextString[myEndStoryState];
        myAudioSource.PlayOneShot(myTalkClips[myEndStoryState]);

        Invoke("FadeOutEndStoryText", 2.0f);
    }

    void FadeOutEndStoryText()
    {
        myStoryText.CrossFadeAlpha(0.0f, 1.0f, false);
        myEndStoryState++;

        Invoke("SetEndStoryText", 2.0f);
    }
}
