using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    Transform gold;
    float f_RotSpeed = 5.0f, f_MoveSpeed = 1.0f;
    public MobilePlayer mobilePlayer;
    public float waitTime;
    public float startWaitTime;

    int multiX = 0;
    int multiY = 0;

    // Use this for initialization
    void Start()
    {
        waitTime = startWaitTime;


    }

    // Update is called once per frame
    void Update()
    {

        gold = GameObject.FindGameObjectWithTag("Gold").transform;
        if (waitTime <= 0)
        {

            var directionToEnemy = gold.transform.position - mobilePlayer.transform.position;

            Vector3 forward = mobilePlayer.transform.TransformDirection(Vector3.forward);
            Vector3 right = mobilePlayer.transform.TransformDirection(Vector3.forward);

            var projectionUP = Vector3.Dot(forward, directionToEnemy);
            var projectionRight = Vector3.Dot(right, directionToEnemy);

            if (projectionRight < 0)
            {
                mobilePlayer.LeftButtonPressed();
                waitTime = startWaitTime;
            }
            else
            {
                mobilePlayer.RightButtonPressed();
                waitTime = startWaitTime;
            }

            if (projectionUP < 0)
            {
                mobilePlayer.LeftButtonPressed();
                waitTime = startWaitTime;
            }
            else
            {
                mobilePlayer.StraightButtonPressed();
                waitTime = startWaitTime;
            }


        }
        else
        {
            waitTime -= Time.deltaTime;

        }
    }
}
