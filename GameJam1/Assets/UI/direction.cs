using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class direction : MonoBehaviour
{

    [SerializeField]
    GridLayoutGroup directions;

    [SerializeField]
    GameObject straight;

    [SerializeField]
    GameObject turn;

    public void UpdateItems(int s, int c)
    {
        //foreach (GameObject g in GameObject.FindGameObjectsWithTag("straight"))
        //    Destroy(g);
        //foreach (GameObject g in GameObject.FindGameObjectsWithTag("turn"))
        //    Destroy(g);

        for (int i = 0; i < s; ++i)
        {
            Instantiate(straight, directions.transform);
        }
        for (int i = 0; i < c; ++i)
        {
            Instantiate(turn, directions.transform);
        }
    }

    public void ChangeItems(MixedBagRandom.Types remove, MixedBagRandom.Types add)
    {
        foreach (Transform t in directions.transform)
        {
            if (t.gameObject.CompareTag((remove == MixedBagRandom.Types.STRAIGHT) ? "straight" : "turn"))
            {
                Destroy(t.gameObject);
                break;
            }
        }
        //Destroy(GameObject.FindGameObjectWithTag((remove == MixedBagRandom.Types.STRAIGHT) ? "straight" : "turn"));

        Instantiate(add == MixedBagRandom.Types.STRAIGHT ? straight : turn, directions.transform);
    }
}
