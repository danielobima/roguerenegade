using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string clothesPath = Application.persistentDataPath + "/characterCustomization.rogue";
    public static void SaveClothes(ClothSaveData clothSaveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
       
        FileStream stream = new FileStream(clothesPath, FileMode.Create);

        formatter.Serialize(stream, clothSaveData);
        stream.Close();
    }
    public static ClothSaveData loadClothes()
    {
        if (File.Exists(clothesPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(clothesPath, FileMode.Open);
            ClothSaveData clothSaveData = (ClothSaveData) formatter.Deserialize(stream);
            stream.Close();

            return clothSaveData;

        }
        else
        {
            Debug.Log("No save");
            return null;
        }
    }
}
