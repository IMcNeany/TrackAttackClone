using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MixedBagRandom : MonoBehaviour
{
    public enum Types
    {
        STRAIGHT,
        CURVED
    }

    public int straight_count = 0;
    public int curved_count = 0;

    public List<Types> bag = new List<Types>();

    public Types GetNewType()
    {
        Types last_type = bag[bag.Count - 1];

        bag.RemoveAt(bag.Count - 1);

        if(bag.Count == 0)
        {
            GenerateNewBag();
        }

        return last_type;
    }

    public void GenerateNewBag()
    {
        PopulateBag();
        RandomiseBag();
    }

    private void PopulateBag()
    {
        bag.Clear();

        for (int i = 0; i < straight_count; ++i)
            bag.Add(Types.STRAIGHT);
        
        for(int i = 0; i < curved_count; ++i)
            bag.Add(Types.CURVED);
    }

    private void RandomiseBag()
    {
        bag = bag.OrderBy(x => Random.value).ToList();
    }

    void Awake()
    {
        GenerateNewBag();
    }
}
