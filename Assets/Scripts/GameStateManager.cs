using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int noOfRows;
    public int noOfColumns;
    public int matchCount;
    public int turnsCount;
    public List<CardData> cards = new List<CardData>();
}

[System.Serializable]
public class CardData
{
    public int cardID;          
    public bool isMatched;     
    public bool isFlipped;   
    public int spriteIndex;
}


public class GameStateManager : MonoBehaviour
{
    private string saveFileName = "SaveGame.json";
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"Save file path: {saveFilePath}");
    }

    public void SaveGame(SaveData saveData)
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Game successfully saved: {json}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    public SaveData LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("Save file not found. Starting a new game.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);
            Debug.Log($"Loaded game data: {json}");
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
            return null;
        }
    }

    public void ClearSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.LogError("No save file found to delete.");
        }
    }
}

