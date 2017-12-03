using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    float myShotDelay = 0.12f;
    float myInitialShotDelay = 0.12f;
    bool myCanShoot = true;

    GameObject myBulletPrefab = null;

    AudioClip myShootClip = null;
    AudioSource myAudioSource = null;

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        myShootClip = Resources.Load<AudioClip>("bullet-shot");
        myBulletPrefab = Resources.Load<GameObject>("Bullet");
    }
	
	void Update()
    {
        myShotDelay -= Time.deltaTime;
        if (myShotDelay <= 0.0f)
        {
            myCanShoot = true;
            myShotDelay = myInitialShotDelay;
        }
    }

    public void Shoot(Transform aTransform)
    {
        if (myCanShoot)
        {
            Vector3 position = aTransform.position;
            position.z = 1.0f;

            GameObject newBullet = Instantiate(myBulletPrefab, position, aTransform.rotation) as GameObject;
            newBullet.name = "Bullet";

            myCanShoot = false;

            myShotDelay = myInitialShotDelay;

            myAudioSource.PlayOneShot(myShootClip, 0.8f);
        }
    }
}
