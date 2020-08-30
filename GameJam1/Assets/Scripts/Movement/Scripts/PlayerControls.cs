using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls
{
    public KeyCode actionButton;
    public KeyCode actionButton2;
    public string HorizontalAxis;
    public string VerticalAxis;
    bool upPressed = false;
    bool downPressed = false;
    bool leftPressed = false;
    bool rightPressed = false;

    public bool GetActionButton()
    {
        return Input.GetKeyDown(actionButton);
    }

    public bool GetAction2Button()
    {
        return Input.GetKeyDown(actionButton2);
    }

    public bool GetUpButton()
    {
        if (Input.GetAxisRaw(VerticalAxis) > 0 && !upPressed)
        {
            upPressed = true;
            return true;
        }

        if (!(Input.GetAxisRaw(VerticalAxis) > 0))
        {
            upPressed = false;
        }
        return false;
    }

    public bool GetDownButton()
    {
        if (Input.GetAxisRaw(VerticalAxis) < 0 && !downPressed)
        {
            downPressed = true;
            return true;
        }

        if (!(Input.GetAxisRaw(VerticalAxis) < 0))
        {
            downPressed = false;
        }
        return false;
    }

    public bool GetLeftButton()
    {
        if (Input.GetAxisRaw(HorizontalAxis) < 0 && !leftPressed)
        {
            leftPressed = true;
            return true;
        }

        if (!(Input.GetAxisRaw(HorizontalAxis) < 0))
        {
            leftPressed = false;
        }
        return false;
    }

    public bool GetRightButton()
    {
        if (Input.GetAxisRaw(HorizontalAxis) > 0 && !rightPressed)
        {
            rightPressed = true;
            return true;
        }

        if (!(Input.GetAxisRaw(HorizontalAxis) > 0))
        {
            rightPressed = false;
        }
        return false;
    }
}

public class PlayerInput : PlayerControls
{
    public PlayerInput(int player)
    {
        switch (player)
        {
            case 1:
                actionButton = KeyCode.Joystick1Button0;
                actionButton2 = KeyCode.Joystick1Button1;
                HorizontalAxis = "joyXAxis1";
                VerticalAxis = "joyYAxis1";
                break;
            case 2:
                actionButton = KeyCode.Joystick2Button0;
                actionButton2 = KeyCode.Joystick2Button1;
                HorizontalAxis = "joyXAxis2";
                VerticalAxis = "joyYAxis2";
                break;
            case 3:
                actionButton = KeyCode.Joystick3Button0;
                actionButton2 = KeyCode.Joystick3Button1;
                HorizontalAxis = "joyXAxis3";
                VerticalAxis = "joyYAxis3";
                break;
            case 4:
                actionButton = KeyCode.Joystick4Button0;
                actionButton2 = KeyCode.Joystick4Button1;
                HorizontalAxis = "joyXAxis4";
                VerticalAxis = "joyYAxis4";
                break;
        }        
    }
}

