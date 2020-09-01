using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ARCompatiablity : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ARSession m_Session;
    public GameObject AR;
    public GameObject nonAR;
    public ClientNetwork network;
    public bool networking = false;
    bool stateCompatiable = false;
  

    IEnumerator Start()
    {
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if(networking)
        {
            if (ARSession.state == ARSessionState.Unsupported)
            {
                stateCompatiable = false;
                network.AR_avaliable = false;

            }
            else
            {
                // Start the AR session
                m_Session.enabled = true;
                stateCompatiable = true;
                network.AR_avaliable = true;


            }
        }
        else
        {
            if (ARSession.state == ARSessionState.Unsupported)
            {
                AR.SetActive(false);
                nonAR.SetActive(true);
                stateCompatiable = false;


            }
            else
            {
                // Start the AR session
                m_Session.enabled = true;

                AR.SetActive(true);
                nonAR.SetActive(false);
                stateCompatiable = true;


            }
        }
    }
}
