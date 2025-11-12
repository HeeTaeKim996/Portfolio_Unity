using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene_CameraManager : MonoBehaviour
{
    private int controllState = 0;


    private void OnDisable()
    {
        if(GameManager.instance != null)
        {
            GameManager.instance.cinemachineController.ResetOffsetCircling();

            if (!GameManager.instance.cinemachineController.cinemachieCam.enabled)
            {
                GameManager.instance.cinemachineController.cinemachieCam.enabled = true;
            }
        }

        controllState = 0;
    }

    public void UpdateControllState()
    {
        if(controllState >= 1)
        {
            controllState = 0;
        }
        else
        {
            controllState++;
        }

        if(controllState == 0)
        {
            if (!GameManager.instance.cinemachineController.cinemachieCam.enabled)
            {
                GameManager.instance.cinemachineController.cinemachieCam.enabled = true;
            }
        }
        else if(controllState == 1)
        {
            GameManager.instance.cinemachineController.cinemachieCam.enabled = false;
        }
    }
    public void CircleControllerUpdate(Vector2 refinedVector)
    {
        if(controllState == 0)
        {
            GameManager.instance.cinemachineController.ChangeMOffsetCircling(refinedVector.x);
        }
        else if(controllState == 1)
        {
            GameManager.instance.cinemachineController.MoveMainCam(refinedVector);
        }
    }

    public void UpDownSliderUpdate(float value)
    {
        if(controllState == 1)
        {
            GameManager.instance.cinemachineController.MoveMainCamUpDown(value);
        }
    }
    public void MainCamForwardAngleUpdate(Vector2 forwardVec)
    {
        if(controllState == 1)
        {
            GameManager.instance.cinemachineController.ChangeMainCamForwardAngle(forwardVec);
        }
    }
}
