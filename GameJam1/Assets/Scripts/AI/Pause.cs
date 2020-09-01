using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public static bool m_paused = false;
    public GameObject m_pauseUI;

    void Awake()
    {
        m_pauseUI.SetActive(true);
        Time.timeScale = 0f;
        m_paused = true;
    }

    public void PauseButtonPress()
    {
        if (m_paused)
        {
            Resume();
        }
        else
        {
            PauseGame();
        }
    }

    public void Resume()
    {
        m_pauseUI.SetActive(false);
        Time.timeScale = 1f;
        m_paused = false;
    }

    public void PauseGame()
    {
        m_pauseUI.SetActive(true);
        Time.timeScale = 0f;
        m_paused = true;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
