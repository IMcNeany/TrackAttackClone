using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{

    #region Singleton

    public static GoldManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject gold;
}
