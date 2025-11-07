
using UnityEngine;

[CreateAssetMenu(fileName = "ShoesData", menuName = "Scriptable/ShoesData")]
public class ShoesData : ScriptableObject
{
    [Header("Path")]
    public string scriptPath;

    [Header("Other")]
    public string shoesName;
    public Mesh shoesMesh;
    public Material[] shoesMaterials;
    public float weight;
    public float defense;
}
