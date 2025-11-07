using UnityEngine;

[CreateAssetMenu(fileName = "UpperBodyClothingSet", menuName = "Scriptable/UpperBodyClothingSet")]
public class UpperBodyClothingSet : ScriptableObject
{
    [Header("Path")]
    public string scriptablePath;

    [Header("Other")]
    public string upperClotheName;

    public Mesh upperBodyMesh;
    public Material[] upperbodyMaterals;
    public float weight;
    public float defense;
}
