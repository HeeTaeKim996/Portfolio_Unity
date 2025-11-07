using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // ==>> 리스트에 사용될 클래스라면, 좌측을 입력해야 리스트로 인스펙터에 입력이 가능함
public class WorldChunkData
{
    public Vector3 position;
    public GameObject prefab;
}
