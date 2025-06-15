using Assets.GracesScripts.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveGameUtility : MonoBehaviour
{
    public static bool HasSaveData => PlayerPrefs.HasKey(SaveKey);

    public static void Load()
    {
        string json = PlayerPrefs.GetString(SaveKey);

        // If no save data, then new game started, don't restore, just return.
        if (string.IsNullOrEmpty(json)) return;

        var saveData = JsonUtility.FromJson<SerializationWrapper>(json).ToDictionary();

        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().Where(x => x is not PlayerDungeon);
        foreach (var saveable in saveables)
        {
            if (saveData.TryGetValue(saveable.UniqueId, out var state))
            {
                saveable.RestoreState(state); // This has the correct data.
            }
        }
    }

    public static Weapon GetDefaultHands()
    {
        var hands = Resources.Load<Weapon>("Items/Weapon/Hands");
        MyGuard.IsNotNull(hands);
        return hands;
    }

    private const string SaveKey = "SceneSaveData";

    public static void SaveGame()
    {
        var saveables = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        var saveData = new Dictionary<string, object>();

        foreach (var saveable in saveables)
        {
            saveData[saveable.UniqueId] = saveable.CaptureState();
        }

        string json = JsonUtility.ToJson(new SerializationWrapper(saveData));
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("At some point need to save containers contents so they dont reload");
        Debug.Log("at some point also need to save converstaions with NPCS? maybe? I won't have backtracking though so idk.");
        Debug.Log("at some point also save if character has been battled and what conversation they are up to and remove SceneAfterWin on Unit_NPC.");

        PlayerPrefs.SetString(SaveKeys.LastScene, SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();

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

        public SerializationWrapper(Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(JsonUtility.ToJson(kvp.Value));
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = JsonUtility.FromJson<object>(values[i]); // Replace with proper typed deserialization
            }
            return dict;
        }
    }

    public static class SaveKeys
    {
        public const string LastScene = "LastScene";

        // TODO Dialogue log should be saved and chests current content.
        // public const string DialogueLog = "Dialogue";
    }
}