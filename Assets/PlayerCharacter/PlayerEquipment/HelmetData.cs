
using UnityEngine;

[CreateAssetMenu(fileName = "HelmetData", menuName = "Scriptable/HelmetData")]
public class HelmetData : ScriptableObject
{
    [Header("Path")]
    public string scriptPath;

    [Header("Other")]
    public string helmetName;
    public Mesh helmetMesh;
    public Material[] helmetMaterials;
    public float weight;
    public float defense;  
}

