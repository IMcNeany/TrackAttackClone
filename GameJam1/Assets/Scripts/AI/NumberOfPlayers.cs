using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberOfPlayers : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyScore;
    bool m_Activate;

    // Start is called before the first frame update
    void Start()
    {
        m_Activate = true;
        enemy.gameObject.SetActive(m_Activate);
        enemyScore.gameObject.SetActive(m_Activate);
    }

    public void Enemy1()
    {
        if (m_Activate == false)
        {
            enemy.gameObject.SetActive(true);
            enemyScore.gameObject.SetActive(true);
            m_Activate = true;
        }
        else
        {
            enemy.gameObject.SetActive(false);
            enemyScore.gameObject.SetActive(false);
            m_Activate = false;
        }
    }

}
