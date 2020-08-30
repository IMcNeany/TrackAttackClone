using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash : MonoBehaviour
{
    [SerializeField]
    private int m_goldCount = 0;
    [SerializeField]AudioClip a_collection;

    public MobilePlayer manager;
    public int id = 0;

    public int UpdateStash(int goldCount)
    {
        if (goldCount >0)
        {
            AudioSource.PlayClipAtPoint(a_collection, Vector3.zero);
            Debug.Log("Should play audio here");
            Animator anim = GetComponentInChildren<Animator>();
            GetComponentInChildren<ParticleSystem>().Play();
            anim.SetTrigger("AddGold");
        }
        m_goldCount += goldCount;

       // manager.CheckEndGame(m_goldCount, id);
        return m_goldCount;
    }
}
