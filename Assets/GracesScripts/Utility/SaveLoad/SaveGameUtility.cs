using Assets.GracesScripts.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveGameUtility : MonoBehaviour
{
    public static bool HasSaveData => PlayerPrefs.HasKey(SaveKeys.SaveKey);

    public static void Load()
    {
        string json = PlayerPrefs.GetString(SaveKeys.SaveKey);

        // If no save data, then new game started, don't restore, just return.
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        var saveData = JsonUtility.FromJson<SerializationWrapper>(json).ToDictionary(); // Dictionary<string, string>

        foreach (var saveable in saveables)
        {
            if (saveData.TryGetValue(saveable.UniqueId, out var jsonString))
            {
                saveable.RestoreState(jsonString); // pass JSON string, not object
            }
        }
    }

    public static Weapon GetDefaultHands()
    {
        var hands = Resources.Load<Weapon>("Items/Weapon/Hands");
        MyGuard.IsNotNull(hands);
        return hands;
    }

    public static void SaveGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        Dictionary<string, string> saveData = new();

        foreach (var saveable in saveables)
        {
            var data = saveable.CaptureState();
            saveData[saveable.UniqueId] = JsonUtility.ToJson(data);
        }

        string json = JsonUtility.ToJson(new SerializationWrapper(saveData));
        PlayerPrefs.SetString(SaveKeys.SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("At some point need to save containers contents so they dont reload");
        Debug.Log("at some point also need to save converstaions with NPCS? maybe? I won't have backtracking though so idk.");
        Debug.Log("at some point also save if character has been battled and what conversation they are up to and remove SceneAfterWin on Unit_NPC.");

        PlayerPrefs.SetString(SaveKeys.LastScene, SceneManager.GetActiveScene().name);

        var saveText = FindObjectOfType<SavedAnimationText>();
        if (saveText == null)
        {
            Debug.LogWarning("not sure why savetext is null idk if it should be or not.");
            return;
        }

        saveText.PlaySavedAnimation();
    }

    [System.Serializable]
    private class SerializationWrapper
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>(); // JSON strings

        public SerializationWrapper(Dictionary<string, string> dict)
        {
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = values[i]; // values[i] is already a JSON string
            }
            return dict;
        }
    }

    /// <summary>
    /// Then the save Keys can be different scenes so i can store per-scene save data. instead of the one "SaveKey"
    /// </summary>
    public static class SaveKeys
    {
        public const string LastScene = "LastScene";
        public const string SaveKey = "SceneSaveData";

        // TODO Dialogue log should be saved and chests current content.
        // public const string DialogueLog = "Dialogue";
    }
}