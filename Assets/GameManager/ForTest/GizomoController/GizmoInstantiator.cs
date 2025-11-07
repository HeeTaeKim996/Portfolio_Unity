using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo_SphereInstantiator : MonoBehaviour
{
    private PolygonType polygonType = PolygonType.sphere;

    private Vector3 sphereCenter;
    private float radius;

    private Vector3 boxPositon;
    private Vector3 boxSize;
    private Vector3 boxRotation;
    

    private void Start()
    {
        ForTestManager.instance.gizmoController.polygonTypeCastEvent += OnPolygonTypeChange;
        ForTestManager.instance.gizmoController.sphereGizmoEvent += OnGizmoSphereInstantiate;
        ForTestManager.instance.gizmoController.boxGizmoEvent += OnBoxGizmoEvent;
    }

    private void OnGizmoSphereInstantiate(Vector3 givenVector, float givenRadius)
    {
        sphereCenter = transform.position + givenVector;
        radius = givenRadius;
    }
    private void OnBoxGizmoEvent(Vector3 position, Vector3 size, Vector3 rotation)
    {
        boxPositon = transform.position + position;
        boxSize = size;
        boxRotation = rotation;
    }

    private void OnDrawGizmos()
    {
        if (polygonType == PolygonType.sphere)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sphereCenter, radius);
        }
        else if(polygonType == PolygonType.box)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(boxPositon, Quaternion.Euler(boxRotation), Vector3.one); // Gizmos.DrawWireCube에서는 로테이션값을 지정할 수 없고, Qaternion.Identity고정이기 때문에, 이렇게 수동으로 조정 처리
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }

    public void OnPolygonTypeChange(PolygonType newPolygonType)
    {
        polygonType = newPolygonType;
    }
}
