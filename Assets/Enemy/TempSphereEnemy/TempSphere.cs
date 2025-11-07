using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSphere : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
        Vector3 keepingTransform = playerHealth.transform.position + new Vector3(0, 2.5f, 6);
        transform.position = keepingTransform;
    }
}
