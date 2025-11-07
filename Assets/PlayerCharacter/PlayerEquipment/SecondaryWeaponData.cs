// #SecondaryWeaponData

using UnityEngine;


[CreateAssetMenu(fileName = "SecondaryWeaponData", menuName = "Scriptable/SecondaryWeaponData")]
public class SecondaryWeaponData : ScriptableObject
{
    [Header("Path")]
    public string scriptablePath;


    [Header("Transform")]
    public string secondaryWeaponName;
    public string modelPath;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;

    [Header("Amount")]
    public float Weight;
    public int shieldPower;

}
