using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTextDisplay : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int _waitingTime;

    private void Awake()
    {
        _waitingTime = 5;
        _text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        while (_waitingTime > 0)
        {
            _waitingTime -= 1;
            _text.text = " Loading... " + _waitingTime;
            yield return new WaitForSecondsRealtime(1);
        }

        if (gameObject.activeSelf)
        {
            NetworkGameManager nm = FindObjectOfType<NetworkGameManager>();
            nm.ChangeControlsState(true);
            nm.loading = false;
            gameObject.SetActive(false);
        }
    }
}