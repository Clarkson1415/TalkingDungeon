using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class PlayerDataUtility
{
    public static void SaveGame(PlayerDungeon player)
    {

#if UNITY_EDITOR
        PlayerPrefs.SetString(SaveKeys.Scene, SceneManager.GetActiveScene().name);
# endif

        PlayerPrefs.SetString(SaveKeys.Scene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosX, player.gameObject.transform.position.x);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosY, player.gameObject.transform.position.y);
        PlayerPrefs.SetFloat(SaveKeys.CurrentWellbeing, player.currentWellbeing);
        PlayerPrefs.SetFloat(SaveKeys.MaxWellbeing, player.maxWellbeing);

        // save path of scriptableobjects
        string equippedWeaponPath = player.equippedWeapon?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedWeaponPath, equippedWeaponPath);

        string equippedClothingPath = player.equippedClothing?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedClothingPath, equippedClothingPath);

        string equippedItemPath = player.equippedSpecialItem?.Path ?? string.Empty;
        PlayerPrefs.SetString(SaveKeys.EquippedItemPath, equippedItemPath);

        List<string> InventoryItemsNames = player.Inventory.Select(item => item != null ? item.Path ?? string.Empty : string.Empty).ToList();
        string itemJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = InventoryItemsNames });
        PlayerPrefs.SetString(SaveKeys.InventoryItemsPaths, itemJson);

        List<string> AbilityNames = player.abilities.Select(ability => ability != null ? ability.Path ?? string.Empty : string.Empty).ToList();
        string abilitiesJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = AbilityNames });
        PlayerPrefs.SetString(SaveKeys.AbilitiesPaths, abilitiesJson);

        // TODO save all ItemContainers in scene. they each will have their own save method to reutrn their json list of items. with by the key: sceneName/ChestName
        // ItecmContainer.Save

        PlayerPrefs.Save();

        // TODO show loading text for a second
    }

    [Serializable]
    public class StringListWrapper
    {
        public List<string> savedStrings;
    }

    public static class SaveKeys
    {
        public const string Scene = "Scene";

#if UNITY_EDITOR
        /// <summary>
        /// Keep track of scenes in testing playthrough
        /// </summary>
        public const string ScenesTraversed = "ScenesTraversed";
# endif
        public const string PlayerPosX = "PlayerPosX";
        public const string PlayerPosY = "PlayerPosY";
        public const string CurrentWellbeing = "CurrentWellbeing";
        public const string MaxWellbeing = "MaxWellbeing";
        public const string InventoryItemsPaths = "InventoryItemsPaths";
        public const string EquippedWeaponPath = "EquippedWeaponPath";
        public const string EquippedClothingPath = "EquippedClothingPath";
        public const string EquippedItemPath = "EquippedItemPath";

        public const string AbilitiesPaths = "AbilitiesPaths";

        // TODO Dialogue log should be saved
        // public const string DialogueLog = "Dialogue";
    }

    public static void LoadFromSave(PlayerDungeon player)
    {
        LoadSaveDataFromLastScene(player);

        // the other stuff except scene already done
        player.gameObject.transform.position = new Vector3(
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosX),
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosY),
            player.gameObject.transform.position.z);
    }

    /// <summary>
    /// Saved in scene 1 then loaded scene 2 then this is called before setupPlayer() to load all the player values inventory etc.
    /// </summary>
    /// <param name="player"></param>
    public static void LoadSaveDataFromLastScene(PlayerDungeon player)
    {
#if UNITY_EDITOR
        var scenes = PlayerPrefs.GetString(SaveKeys.ScenesTraversed);
        player.scenesTraversed.Add(scenes);
# endif

        // deserialize saved items.
        string itemsJson = PlayerPrefs.GetString(SaveKeys.InventoryItemsPaths);
        player.Inventory = DeserializeSavedStrings<Item>(itemsJson);

        // deserialize saved abilities
        string abilitiesJson = PlayerPrefs.GetString(SaveKeys.AbilitiesPaths);
        player.abilities = DeserializeSavedStrings<Ability>(abilitiesJson);

        player.currentWellbeing = PlayerPrefs.GetFloat(SaveKeys.CurrentWellbeing);
        player.maxWellbeing = PlayerPrefs.GetFloat(SaveKeys.MaxWellbeing);

        string weaponPath = PlayerPrefs.GetString(SaveKeys.EquippedWeaponPath);
        if (!string.IsNullOrEmpty(weaponPath))
        {
            player.equippedWeapon = Resources.Load<Item>(weaponPath);
        }

        string clothingPath = PlayerPrefs.GetString(SaveKeys.EquippedClothingPath);
        if (!string.IsNullOrEmpty(clothingPath))
        {
            player.equippedClothing = Resources.Load<Item>(PlayerPrefs.GetString(SaveKeys.EquippedClothingPath));
        }

        string specialItemPath = PlayerPrefs.GetString(SaveKeys.EquippedClothingPath);
        if (!string.IsNullOrEmpty(specialItemPath))
        {
            player.equippedSpecialItem = Resources.Load<Item>(PlayerPrefs.GetString(SaveKeys.EquippedItemPath));
        }
    }

    private static List<T> DeserializeSavedStrings<T>(string json) where T : ScriptableObject
    {
        var wrapper = JsonUtility.FromJson<StringListWrapper>(json);

        List<T> loadedItems = new List<T>();
        foreach (string itemPath in wrapper.savedStrings)
        {
            T item = Resources.Load<T>(itemPath);
            if (item != null)
                loadedItems.Add(item);
        }

        return loadedItems;
    }
}