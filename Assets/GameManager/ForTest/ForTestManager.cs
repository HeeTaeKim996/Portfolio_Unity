using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTestManager : MonoBehaviour
{
    [HideInInspector]
    public static ForTestManager instance;
    [HideInInspector]
    public event Action<Vector3, float> OnGizmoSphereInstantiate;

    [HideInInspector]
    public bool isSpehererGizmoOn;
    [HideInInspector]
    public ForTest_ArbitraryValueController arbitraryController;
    [HideInInspector]
    public GizmoController gizmoController;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        arbitraryController = GetComponentInChildren<ForTest_ArbitraryValueController>();
        gizmoController = GetComponentInChildren<GizmoController>();
    }


    private void Update()
    {

    }

    public void GizomoSphereInstantiate(Vector3 position, float radius)
    {
        OnGizmoSphereInstantiate?.Invoke(position, radius);
    }
}
