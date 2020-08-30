using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScene : MonoBehaviour
{
    public Material m_transitionMaterial;

    public int m_nextScene = 0;

    public float speed = 1.0f;
    public float timer = 0.0f;

    public bool m_transitionOut = true;
    
    void Update()
    {
        timer += Time.deltaTime * speed;

        if(timer > 1.0f)
        {
            if (m_transitionOut)
                SceneManager.LoadScene(m_nextScene);
            else
                Destroy(this);
        }
    }

    [ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_transitionMaterial.SetFloat("_Timer", m_transitionOut ? (timer) : (1 - timer));
        
        Graphics.Blit(source, destination, m_transitionMaterial);
    }
}
