using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] int m_gold;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Mine(Cart player)
    {
        player.addGold(m_gold);
        Destroy(gameObject);
    }

    void goldDepleted()
    {

    }
}
