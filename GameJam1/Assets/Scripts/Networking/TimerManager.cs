using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private Image _timerFill;

    // Start is called before the first frame update
    [SerializeField] private int _duration;
    [SerializeField] private ClientGoldManager _clientGoldManager;

    private void Awake()
    {
        _duration = -1;
    }

    public void UpdateTimerUI(int seconds)
    {
        if (_duration == -1)
        {
            _duration = seconds;
        }

        _timerText.text = seconds.ToString();

        float timeRatio = (float) seconds / _duration;
        _timerFill.fillAmount = timeRatio;
    }
}