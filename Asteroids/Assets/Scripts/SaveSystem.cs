using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveData(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Asteroid.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData saveData = new GameData();
        saveData.UpdateDataByData(GameController.Instance.Data);

        formatter.Serialize(stream, saveData);
        stream.Close();
     }

    public static GameData LoadData()
    {
        GameData loadData = null;

        string path = Application.persistentDataPath + "/Asteroid.data";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            loadData = (GameData)formatter.Deserialize(stream);

            stream.Close();
        }

        return loadData;
    }
}
