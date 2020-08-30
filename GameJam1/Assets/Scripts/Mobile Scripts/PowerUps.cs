using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public float duration = 10.0f;
    public float multiplier = 2.0f;

    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(PickUp(other));
        }
    }

    IEnumerator PickUp(Collider player)
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        MobilePlayer stats = player.GetComponent<MobilePlayer>();
        stats.speed *= multiplier;

        yield return new WaitForSeconds(duration);

        stats.speed /= multiplier;
        Destroy(gameObject);
    }
}
