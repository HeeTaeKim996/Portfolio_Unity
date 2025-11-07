using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPositionTester : MonoBehaviour
{
    public GameObject playerStartPositionObject;


    [HideInInspector]
    public int startType = 0;

    public Vector3 ReturnPlayerStartPoint()
    {
        if(startType == 0)
        {
            return playerStartPositionObject.transform.position;
        }
        else if(startType == 1)
        {
            return GameManager.instance.databaseManager.LoadPlayerPosition();
        }
        else
        {
            return Vector3.zero;
        }
    }
}
