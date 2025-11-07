using System.Collections;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/PlayerStatData.json";
    private static string equipmentSaveFilePath = Application.persistentDataPath + "/EquipmentSaveData.json";

    // @@ PlayerStatDataJson
    public static void SavePlayerJsonData(PlayerJsonSaveData jsonData)
    {
        string json = JsonUtility.ToJson(jsonData, true);
        File.WriteAllText(saveFilePath, json);
    }

    public static PlayerJsonSaveData LoadPlayerJsonData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerJsonSaveData jsonData = JsonUtility.FromJson<PlayerJsonSaveData>(json);
            return jsonData;
        }
        else
        {
            return new PlayerJsonSaveData();
        }
    }
    public static void ResetPlayerJsonData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }


    // @@ EquipmentSaveDataJson
    public static void SaveEquipmentJsonData(EquipmentJsonSaveData jsonData)
    {
        string json = JsonUtility.ToJson(jsonData, true);

        File.WriteAllText(equipmentSaveFilePath, json);
    }

    public static EquipmentJsonSaveData LoadEquipmentJsonData()
    {
        if (File.Exists(equipmentSaveFilePath))
        {
            string json = File.ReadAllText(equipmentSaveFilePath);
            EquipmentJsonSaveData jsonData = JsonUtility.FromJson<EquipmentJsonSaveData>(json);
            return jsonData;
        }
        else
        {
            return new EquipmentJsonSaveData();
        }
    }

    public static void ResetEquipmentJsonData()
    {
        if (File.Exists(equipmentSaveFilePath))
        {
            File.Delete(equipmentSaveFilePath);
        }
    }
}

