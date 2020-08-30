using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    Text m_text;
    public AudioClip m_fanfare;

    float m_fontGoalSize;

    public Material m_transitionMaterial;
    public AnimationCurve m_fontCurve;
    float m_counter = 0.0f;

    public bool m_start = false;
    public bool m_playOnce = true;

    public void Win(int playerID)
    {
        m_start = true;

        m_text.text = "Player " + playerID + " wins!";
    }

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
        m_text.text = "";
        m_fontGoalSize = m_text.fontSize;
        m_text.fontSize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_start)
        {
            m_counter += Time.deltaTime;

            m_text.fontSize = (int) (m_fontGoalSize * m_fontCurve.Evaluate(m_counter));

            if(m_counter > 1.0f && m_playOnce)
            {
                m_playOnce = false;

                AudioSource.PlayClipAtPoint(m_fanfare,Camera.main.transform.position);
            }
            if(m_counter > 3.0f)
            { 
                TransitionScene t = Camera.main.gameObject.AddComponent<TransitionScene>();

                t.m_nextScene = 0;
                t.m_transitionOut = true;
                t.m_transitionMaterial = m_transitionMaterial;
            }
        }       
    }
}