using UnityEngine;

[CreateAssetMenu(fileName = "UnderClotheData", menuName = "Scriptable/UnderClotheData")]
public class UnderClotheData : ScriptableObject
{
    [Header("Path")]
    public string scriptablePath;

    [Header("Other")]
    public string underClotheName;

    public Mesh underClotheMesh;
    public Material[] underClotheMaterals;
    public float weight;
    public float defense;
}
