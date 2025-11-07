using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene_CameraController : MonoBehaviour
{
    private TestScene_CameraManager cameraManager;

    private float circleProportion = 0.9f;

    // CircleController
    public RectTransform circleControllerTransform;
    private float circleControllerRadius;
    private int activeTouchId = -1;
    private bool didItTouchCircleControllerButton = false;

    private Vector2 originalCircleControllerPosition;
    private Vector2 touchStartPosition;
    public RectTransform smallCircleControllerTransform;
    private float smallCircleRadius;
    private Vector2 touchCurrentPosition;
    private Vector2 refinedCircleVector;

    // cameraForwardController
    public RectTransform forwardRect;
    private float forwardRadius;
    private int activeTouchId2 = -1;
    private bool didItTOuchForwardButton = false;

    private Vector2 forwardTouchPosition;


    private void Awake()
    {
        cameraManager = GetComponent<TestScene_CameraManager>();
        originalCircleControllerPosition = circleControllerTransform.anchoredPosition;

        circleControllerRadius = circleControllerTransform.rect.width / 2f * circleProportion;
        smallCircleRadius = smallCircleControllerTransform.rect.width / 2f * circleProportion;
        forwardRadius = forwardRect.rect.width / 2f * circleProportion;
    }


    private void Update()
    {
        for(int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if(activeTouchId == touch.fingerId || activeTouchId == -1)
            {
                if (touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, circleControllerTransform, circleControllerTransform.rect.center, circleControllerRadius))
                {
                    activeTouchId = touch.fingerId;
                    didItTouchCircleControllerButton = true;
                    touchStartPosition = touch.position;
                    circleControllerTransform.anchoredPosition = touchStartPosition;
                    smallCircleControllerTransform.anchoredPosition = touchStartPosition;
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (didItTouchCircleControllerButton)
                    {
                        touchCurrentPosition = touch.position;
                        Vector2 tempVector = touchCurrentPosition - touchStartPosition;
                        float buffer;

                        if (smallCircleRadius < tempVector.magnitude)
                        {
                            buffer = tempVector.magnitude;
                        }
                        else
                        {
                            buffer = smallCircleRadius;
                        }

                        refinedCircleVector = tempVector / buffer;
                        cameraManager.CircleControllerUpdate(refinedCircleVector);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTouchCircleControllerButton)
                    {
                        didItTouchCircleControllerButton = false;
                        refinedCircleVector = Vector2.zero;
                        circleControllerTransform.anchoredPosition = originalCircleControllerPosition;
                        smallCircleControllerTransform.anchoredPosition = originalCircleControllerPosition;
                        activeTouchId = -1;
                    }
                }
            }
            if(activeTouchId2 == touch.fingerId || activeTouchId2 == -1)
            {
                if(touch.phase == TouchPhase.Began && isTouchWithinCircle(touch.position, forwardRect, forwardRect.rect.center, forwardRadius))
                {
                    activeTouchId2 = touch.fingerId;
                    didItTOuchForwardButton = true;
                    forwardTouchPosition = touch.position;
                }
                else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (didItTOuchForwardButton)
                    {
                        if(touch.position != forwardTouchPosition)
                        {
                            Vector2 forwardVec = touch.position - forwardTouchPosition;
                            cameraManager.MainCamForwardAngleUpdate(forwardVec);

                            forwardTouchPosition = touch.position;
                        }
                    }
                }
                else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (didItTOuchForwardButton)
                    {
                        didItTOuchForwardButton = false;
                        activeTouchId2 = -1;
                    }
                }
            }
        }
    }



    private bool isTouchWithinCircle(Vector2 touchPosition, RectTransform rectTransform, Vector2 circleCenter, float radius)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, null, out localPoint);
        float distance = Vector2.Distance(localPoint, circleCenter);

        return distance <= radius;
    }
}
