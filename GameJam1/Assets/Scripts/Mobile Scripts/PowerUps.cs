using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public float duration = 10.0f;
    public float multiplier = 2.0f;
    public AudioSource audio;

    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Speed(other));

            /*
            int actions = Random.Range(0, 2);

            switch (actions)
            {
                case 0:
                    StartCoroutine(Speed(other));
                    break;

                case 1:
                    StartCoroutine(Invincibility(other));
                    break;
            }
            */
        }
    }

    IEnumerator Speed(Collider player)
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        MobilePlayer stats = player.GetComponent<MobilePlayer>();
        stats.speed *= multiplier;
        audio.Play();

        yield return new WaitForSeconds(duration);

        stats.speed /= multiplier;
        Destroy(gameObject);
    }
    /*
    IEnumerator Invincibility(Collider player)
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        player.GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(duration);

        player.GetComponent<BoxCollider>().enabled = true;
        Destroy(gameObject);
    }
    */
}
