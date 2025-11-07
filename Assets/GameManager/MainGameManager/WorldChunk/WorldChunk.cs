
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> nonClassStuctures = new List<Transform>();
    [HideInInspector]
    public List<Renderer> terrains = new List<Renderer>();

    private void Awake()
    {
        foreach(Transform childTransform in GetComponentsInChildren<Transform>())
        {
            if(childTransform.CompareTag("NonClassStructure"))
            {
                nonClassStuctures.Add(childTransform);
            }

            if (childTransform.CompareTag("Terrain"))
            {
                Renderer renderer = childTransform.GetComponent<Renderer>();
                terrains.Add(renderer);
            }
        }
    }
}
