using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClientPlayer : MonoBehaviour
{
    public Vector3 cart_pos;
    public Vector3 cart_rot;
    public int ID = -1;
    public float test_speed = 10.0f;
    private Text text;

    public bool canMove;


    private void Awake()
    {
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            UpdateCart();
        }
    }

    private void UpdateCart()
    {
        cart_pos = transform.localPosition;
        Vector3 new_rot;
        new_rot.x = transform.localRotation.eulerAngles.x;
        new_rot.y = transform.localRotation.eulerAngles.y;
        new_rot.z = transform.localRotation.eulerAngles.z;
        cart_rot = new_rot;
    }
}