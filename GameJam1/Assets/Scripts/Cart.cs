using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public int m_score;
    public int m_scorebag;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        //check we're colliding with gold
        Gold checkGold = other.gameObject.GetComponent<Gold>();
        if (checkGold)
        {
            checkGold.Mine(this);
        }

        if (other.CompareTag("Stash"))
        {
            Score();
        }
    }


    /// <summary>
    /// Cash in points
    /// </summary>
    public void Score()
    {
        m_score += m_scorebag;
        m_scorebag = 0;
    }

    /// <summary>
    /// Collect gold and put it into the scorebag
    /// </summary>
    /// <param name="_gold">Value of gold collected</param>
    public void addGold(int _gold)
    {
        m_scorebag += _gold;
    }
}
