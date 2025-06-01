using Assets.GracesScripts;
using Assets.GracesScripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveGameUtility : MonoBehaviour
{
    /// <summary>
    /// To be used after run away success from battle. Will only save health, items in inventory equipped things. And add anything else that could be change within a battle.
    /// </summary>
    public static void SaveStuffFromBattle(PlayerDungeon player)
    {
        PlayerPrefs.SetFloat(SaveKeys.CurrentWellbeing, player.currentHealth);
        PlayerPrefs.SetFloat(SaveKeys.MaxWellbeing, player.maxHealth);

        // save path of scriptableobjects
        string equippedWeaponPath = player.equippedWeapon?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedWeaponPath, equippedWeaponPath);

        string equippedItemPath = player.equippedSpecialItem?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedItemPath, equippedItemPath);

        List<string> InventoryItemsNames = player.Inventory.Select(item => item != null ? item.Path ?? string.Empty : string.Empty).ToList();
        string itemJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = InventoryItemsNames });
        PlayerPrefs.SetString(SaveKeys.InventoryItemsPaths, itemJson);

        // save that we were in battle scene
        PlayerPrefs.SetString(SaveKeys.LastScene, SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();
    }

    public static void SaveGame(PlayerDungeon player)
    {
        PlayerPrefs.SetString(SaveKeys.LastScene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosX, player.gameObject.transform.position.x);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosY, player.gameObject.transform.position.y);
        PlayerPrefs.SetFloat(SaveKeys.CurrentWellbeing, player.currentHealth);
        PlayerPrefs.SetFloat(SaveKeys.MaxWellbeing, player.maxHealth);

        // save path of scriptableobjects
        string equippedWeaponPath = player.equippedWeapon?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedWeaponPath, equippedWeaponPath);

        string equippedItemPath = player.equippedSpecialItem?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedItemPath, equippedItemPath);

        List<string> InventoryItemsNames = player.Inventory.Select(item => item != null ? item.Path ?? string.Empty : string.Empty).ToList();
        string itemJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = InventoryItemsNames });
        PlayerPrefs.SetString(SaveKeys.InventoryItemsPaths, itemJson);

        // TODO save all ItemContainers in scene. they each will have their own save method to reutrn their json list of items. with by the key: sceneName/ChestName
        // ItecmContainer.Save
        Debug.Log("At some point need to save containers contents so they dont reload");
        Debug.Log("at some point also need to save converstaions with NPCS? maybe? I won't have backtracking though so idk.");

        PlayerPrefs.Save();

        var saveText = FindObjectOfType<SavedAnimationText>();
        if (saveText == null)
        {
            Debug.LogWarning("not sure why savetext is null idk if it should be or not.");
            return;
        }

        saveText.PlaySavedAnimation();
    }

    [Serializable]
    public class StringListWrapper
    {
        public List<string> savedStrings;
    }

    public static class SaveKeys
    {
        public const string LastScene = "Scene";
        public const string PlayerPosX = "PlayerPosX";
        public const string PlayerPosY = "PlayerPosY";
        public const string CurrentWellbeing = "CurrentWellbeing";
        public const string MaxWellbeing = "MaxWellbeing";
        public const string InventoryItemsPaths = "InventoryItemsPaths";
        public const string EquippedWeaponPath = "EquippedWeaponPath";
        public const string EquippedItemPath = "EquippedItemPath";
        public const string GameState = "GameState";

        // TODO Dialogue log should be saved and chests current content.
        // public const string DialogueLog = "Dialogue";
    }

    /// <summary>
    /// Saved in scene 1 then loaded scene 2 then this is called before setupPlayer() to load all the player values inventory etc. but not the position.
    /// </summary>
    /// <param name="player"></param>
    public static void LoadSaveNotPosition(PlayerDungeon player)
    {
        var scenes = PlayerPrefs.GetString(SaveKeys.LastScene);
        if (!string.IsNullOrEmpty(scenes))
        {
            player.scenesTraversed.Add(scenes);
        }

        // deserialize saved items.
        string itemsJson = PlayerPrefs.GetString(SaveKeys.InventoryItemsPaths);
        player.Inventory = DeserializeSavedStrings<DungeonItem>(itemsJson);

        player.currentHealth = PlayerPrefs.GetFloat(SaveKeys.CurrentWellbeing);
        player.maxHealth = PlayerPrefs.GetFloat(SaveKeys.MaxWellbeing);

        string weaponPath = PlayerPrefs.GetString(SaveKeys.EquippedWeaponPath);
        if (!string.IsNullOrEmpty(weaponPath))
        {
            player.equippedWeapon = Resources.Load<Weapon>(weaponPath);
        }

        string specialItemPath = PlayerPrefs.GetString(SaveKeys.EquippedItemPath);
        if (!string.IsNullOrEmpty(specialItemPath))
        {
            player.equippedSpecialItem = Resources.Load<SpecialItem>(PlayerPrefs.GetString(SaveKeys.EquippedItemPath));
        }

        Debug.Log("Loaded from save and position");
    }



    public static void LoadPlayerPosition(PlayerDungeon player)
    {
        player.gameObject.transform.position = new Vector3(
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosX),
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosY),
            player.gameObject.transform.position.z);
    }

    private static List<T> DeserializeSavedStrings<T>(string json) where T : ScriptableObject
    {
        var wrapper = JsonUtility.FromJson<StringListWrapper>(json);

        List<T> loadedItems = new();
        foreach (string itemPath in wrapper.savedStrings)
        {
            T item = Resources.Load<T>(itemPath);
            if (item != null)
                loadedItems.Add(item);
        }

        return loadedItems;
    }
}