using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cinemachine.Utility;
using UnityEngine;

public class GizmoController : MonoBehaviour
{
    public Button_PointerUpDown debugButton;
    public Button_PointerUpDown changePolygonButton;
    public Button_PointerUpDown changeControllButton;

    [HideInInspector]
    public event Action<PolygonType> polygonTypeCastEvent;
    [HideInInspector]
    public PolygonType polygonType = PolygonType.sphere;
    private int controllerInt = 0;
    public List<Slider_UpDownValue> sphereSliders;
    public List<GameObject> gameObjectInstantiators;


    public Vector3 sphereDefaulPositionVector;
    public float sphereDefaultRadius;
    [HideInInspector]
    public event Action<Vector3, float> sphereGizmoEvent;
    private float sphereHorizontal = 0;
    private float sphereVertical = 0;
    private float sphererAltitude = 0;
    private float sphereRadius = 0;
    private Vector3 givingVector;
    private float givingRadius;


    private Vector3 boxChangePosition = Vector3.zero;
    private Vector3 boxChangeSize = Vector3.zero;
    private Vector3 boxChangeRotation = Vector3.zero;
    public Vector3 boxDefaultPosition;
    public Vector3 boxDefaultSize;
    public Vector3 boxDefaultRotation;
    private Vector3 boxGivingPosition;
    private Vector3 boxGivingSize;
    private Vector3 boxGivingRotation;
    [HideInInspector]
    public event Action<Vector3, Vector3, Vector3> boxGizmoEvent;



    private void Awake()
    {
        foreach(Slider_UpDownValue slider in sphereSliders)
        {
            slider.GetSliderValue += GetSliderValue;
        }
        debugButton.OnPressUpButtonEvent += OnPressUpButton;
        changePolygonButton.OnPressUpButtonEvent += OnPressUpButton;
        changeControllButton.OnPressUpButtonEvent += OnPressUpButton;
    }
    private void Start()
    {
        givingVector = sphereDefaulPositionVector;
        givingRadius = sphereDefaultRadius;

        boxGivingPosition = boxDefaultPosition;
        boxGivingSize = boxDefaultSize;
        boxGivingRotation = boxDefaultRotation;

        sphereGizmoEvent?.Invoke(givingVector, givingRadius);

        foreach (GameObject instantiator in gameObjectInstantiators)
        {
            instantiator.SetActive(false);
        }
        gameObjectInstantiators[0].SetActive(true);
    }

    private void GetSliderValue(object sender, float newValue)
    {
        Slider_UpDownValue source = sender as Slider_UpDownValue;   // Slider_UpDownValues 클래스에서 자세히 설명했지만, 이건 포괄타입 object로 받은 값을 다시 구체적인 타입으로 재지정하는 작업

        if (sphereSliders.Contains(source))
        {
            // SphereController
            if (source == sphereSliders[0])
            {
                sphereHorizontal += newValue * 0.1f;
            }
            else if (source == sphereSliders[1])
            {
                sphereVertical += newValue * 0.1f;
            }
            else if (source == sphereSliders[2])
            {
                sphererAltitude += newValue * 0.1f;
            }
            else if (source == sphereSliders[3])
            {
                sphereRadius += newValue * 0.1f;
            }

            // BoxController
            else if (source == sphereSliders[4])
            {
                if (controllerInt == 0)
                {
                    boxChangePosition.x += newValue * 0.1f;
                }
                else if (controllerInt == 1)
                {
                    boxChangeSize.x += newValue * 0.1f;
                }
                else if (controllerInt == 2)
                {
                    boxChangeRotation.x += newValue * 5f;
                }
            }
            else if (source == sphereSliders[5])
            {
                if (controllerInt == 0)
                {
                    boxChangePosition.z += newValue * 0.1f;
                }
                else if (controllerInt == 1)
                {
                    boxChangeSize.z += newValue * 0.1f;
                }
                else if (controllerInt == 2)
                {
                    boxChangeRotation.z += newValue * 5f;
                }
            }
            else if(source == sphereSliders[6])
            {
                if(controllerInt == 0)
                {
                    boxChangePosition.y += newValue * 0.1f;
                }
                else if(controllerInt == 1)
                {
                    boxChangeSize.y += newValue * 0.1f;
                }
                else if(controllerInt == 2)
                {
                    boxChangeRotation.y += newValue * 5f;
                }
            }

            if(polygonType == PolygonType.sphere)
            {
                SphereUpdate(sphereHorizontal, sphereVertical, sphererAltitude, sphereRadius);
            }
            else if(polygonType == PolygonType.box)
            {
                BoxUpdate();
            }
        }
    }

    private void SphereUpdate(float horizon, float vert, float alt, float rad)
    {
        givingVector = new Vector3(horizon, alt, vert) + sphereDefaulPositionVector;
        givingRadius = rad + sphereDefaultRadius;
        sphereGizmoEvent?.Invoke(givingVector, givingRadius);
    }
    private void BoxUpdate()
    {
        boxGivingPosition = boxDefaultPosition + boxChangePosition;
        boxGivingSize = boxDefaultSize + boxChangeSize;
        boxGivingRotation = boxDefaultRotation + boxChangeRotation;
        boxGizmoEvent?.Invoke(boxGivingPosition, boxGivingSize, boxGivingRotation);
    }

    private void OnPressUpButton(Button_PointerUpDown button)
    {
        if(button == debugButton)
        {
            if (polygonType == PolygonType.sphere)
            {
                Debug.Log($"Position : {givingVector}, Rad: {givingRadius}");
            }
            else if(polygonType == PolygonType.box)
            {
                Debug.Log($"Position : {boxGivingPosition}, Size : {boxGivingSize}, Rotation : {boxGivingRotation}");
            }
        }
        else if(button == changePolygonButton)
        {
            ChangePolygonType();
        }
        else if(button == changeControllButton)
        {
            ChangeControllerInt();
        }
    }

    private void ChangePolygonType()
    {
        foreach(GameObject instantiator in gameObjectInstantiators)
        {
            instantiator.SetActive(false);
        }

        if (polygonType == PolygonType.sphere)
        {
            polygonType = PolygonType.box;
            controllerInt = 0;
            gameObjectInstantiators[1].SetActive(true);
            boxGizmoEvent?.Invoke(boxGivingPosition, boxGivingSize, boxGivingRotation);
            Debug.Log(boxGivingSize);
        }
        else if(polygonType == PolygonType.box)
        {
            polygonType = PolygonType.sphere;
            gameObjectInstantiators[0].SetActive(true);
            sphereGizmoEvent?.Invoke(givingVector, givingRadius);
        }

        polygonTypeCastEvent?.Invoke(polygonType);
        Debug.Log($"GizmoController : {polygonType}");
    }


    private void ChangeControllerInt()
    {
        if(polygonType == PolygonType.box)
        {
            if(controllerInt < 2)
            {
                controllerInt++;
            }
            else
            {
                controllerInt = 0;
            }
            Debug.Log($"GizmoController : ControllerInt = {controllerInt}");
        }
    }

}
