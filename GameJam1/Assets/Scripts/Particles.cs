using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponent<ParticleSystem>().isStopped)
        {
            Debug.Log("destory");
            Destroy(this.gameObject);
        }
    }
}
