using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameTimer : MonoBehaviour
{
    [SerializeField] private int _gameLengthInSeconds;

    [SerializeField] private Server _server;


    private void Awake()
    {
        _server = gameObject.GetComponent<Server>();
    }


    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        var timeLeft = _gameLengthInSeconds;

        while (timeLeft > 0)
        {
            _server.SendTimerUpdateToAllClients(timeLeft);
            timeLeft -= 1;
            yield return new WaitForSecondsRealtime(1);
        }

        _server.SendTimerUpdateToAllClients(timeLeft);

        _server.SendGameFinishedNotificationToAllClients(true);
      
        
        yield return new WaitForSeconds(1);
        _server.StopServer();

        yield return new WaitForSeconds(1);
        Destroy(_server.gameObject);
        SceneManager.LoadScene("Start");
    }
}