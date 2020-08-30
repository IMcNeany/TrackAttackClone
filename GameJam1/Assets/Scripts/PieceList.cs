using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceList : MonoBehaviour
{
    MixedBagRandom bag;

    public List<MixedBagRandom.Types> stockpile = new List<MixedBagRandom.Types>();

    public int stockpile_count;

    public direction m_UIDirections;

    int GetStraightCount()
    {
        int c = 0;
        foreach (MixedBagRandom.Types t in stockpile)
            if (t == MixedBagRandom.Types.STRAIGHT) c++;
        return c;
    }

    int GetCurvedCount()
    {
        int c = 0;
        foreach (MixedBagRandom.Types t in stockpile)
            if (t == MixedBagRandom.Types.CURVED) c++;
        return c;
    }

    void Start()
    {
        bag = GetComponent<MixedBagRandom>();

        for(int i = 0; i < stockpile_count; ++i)
        {
            MixedBagRandom.Types newType = bag.GetNewType();

            stockpile.Add(newType);
        }

       // m_UIDirections.UpdateItems(GetStraightCount(), GetCurvedCount());
    }

    public bool GetTrack(MixedBagRandom.Types desired_type)
    {
        foreach(MixedBagRandom.Types type in stockpile)
        {
            if (type == desired_type)
            {
                stockpile.Remove(type);
                MixedBagRandom.Types newType = bag.GetNewType();
                stockpile.Add(newType);

             //   m_UIDirections.ChangeItems(desired_type, newType);

                return true;
            }
        }

        return false;
    }
}
