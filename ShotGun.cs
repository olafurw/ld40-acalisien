using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : MonoBehaviour
{
    float myShotDelay = 0.5f;
    float myInitialShotDelay = 0.5f;
    bool myCanShoot = true;
    
    GameObject myBulletPrefab = null;

    AudioClip myShootClip = null;
    AudioSource myAudioSource = null;

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        myShootClip = Resources.Load<AudioClip>("pellet-shot3");
        myBulletPrefab = Resources.Load<GameObject>("Pellet");
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
            for(int i = 0; i < 8; i++)
            {
                StartCoroutine(CreatePellet(aTransform));
            }

            myCanShoot = false;

            myShotDelay = myInitialShotDelay;

            myAudioSource.PlayOneShot(myShootClip, 0.8f);
        }
    }

    public IEnumerator CreatePellet(Transform aTransform)
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 0.05f));

        Vector3 position = aTransform.position;
        position.z = -1.0f;

        GameObject newBullet = Instantiate(myBulletPrefab, position, aTransform.rotation) as GameObject;
        newBullet.name = "Bullet";

        Bullet bullet = newBullet.GetComponent<Bullet>();
        bullet.myBulletSpeed = 12.5f;
        bullet.myBulletSpeed += Random.Range(-1.0f, 1.0f);

        bullet.myLifetimeSec = 0.6f;
        bullet.myLifetimeSec += Random.Range(-0.2f, 0.2f);
    }
}
