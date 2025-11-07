using System.Collections.Generic;

[System.Serializable]
public class EquipmentJsonSaveData
{
    public List<SlotData> equippedSlots = new List<SlotData>();
    public List<SlotData> inventorySlots = new List<SlotData>();
}
