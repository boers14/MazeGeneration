using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSytem
{
    public static void SaveGame()
    {
        DeleteGame();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "savedGame.maze");
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            PlayerData data = new PlayerData(ScoreManager.instance.value);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    public static PlayerData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.maze");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();
                return data;
            }
        }
        else
        {
            return null;
        }
    }

    public static void DeleteGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.maze");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool CheckIfFileExist()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.maze");
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
