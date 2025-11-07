
using UnityEngine;

[CreateAssetMenu(fileName = "GlovesData", menuName = "Scriptable/GlovesData")]
public class GlovesData : ScriptableObject
{
    [Header("Path")]
    public string scriptPath;

    [Header("Other")]
    public string glovesName;
    public Mesh glovesMesh;
    public Material[] glovesMaterials;
    public float weight;
    public float defense;
}
