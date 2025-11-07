// #IItemData
using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public string modelPath;

    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;
}
