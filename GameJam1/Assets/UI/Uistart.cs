using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Uistart : MonoBehaviour
{

    [SerializeField]
    GameObject P3panel;

    [SerializeField]
    GameObject p3active;

    [SerializeField]
    GameObject P4panel;

    [SerializeField]
    GameObject p4active;

    private int playersactive;

    // Start is called before the first frame update
    void Start()
    {
        playersactive = 2;
    }

    // Update is called once per frame
    void Update()
    {


        //if (player3 keypadactive)
        //{
        //    p3active.SetActive(true);
        //    P3panel.SetActive(false);
        //    playersactive += 1;               
        //}

        //if(player4 keypadactive)
        //{
        //    p4active.SetActive(true);
        //    P4panel.SetActive(false);
        //    playersactive += 1;

        //}




    }


    public void startmatch()
    {
        if(playersactive==2)
        {
            SceneManager.LoadScene(1);

        }
        else if (playersactive==3)
        {

            SceneManager.LoadScene(2);
            //change if we only use 1 scene 
        }
        else if (playersactive==4)
        {
            SceneManager.LoadScene(3);
            //change if we only use 1 scene 

        }




    }




}
