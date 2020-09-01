using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    private static BGMusic _bgMusic;

    void Awake()
    {
        if (_bgMusic == null)
        {
            _bgMusic = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}