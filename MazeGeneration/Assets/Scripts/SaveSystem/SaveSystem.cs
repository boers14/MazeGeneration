using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSytem
{
    // Save the game using the binary formatter so game data cannot be changed easily
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

    // Load the game
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

    // Delete the game
    public static void DeleteGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savedGame.maze");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }


    // Check if there is a saved game
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
