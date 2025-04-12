using EasyTransition;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : DungeonButton
{
    public TransitionSettings transition;

    public void SaveGame()
    {
        var player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            throw new ArgumentNullException("player not found cannot save");
        }

        PlayerPrefs.SetString(SaveKeys.Scene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosX, player.gameObject.transform.position.x);
        PlayerPrefs.SetFloat(SaveKeys.PlayerPosY, player.gameObject.transform.position.y);
        PlayerPrefs.SetFloat(SaveKeys.CurrentWellbeing, player.currentWellbeing);
        PlayerPrefs.SetFloat(SaveKeys.MaxWellbeing, player.maxWellbeing);

        // save path of scriptableobjects
        PlayerPrefs.SetString(SaveKeys.EquippedWeaponPath, player.equippedWeapon.Path);
        PlayerPrefs.SetString(SaveKeys.EquippedClothingPath, player.equippedClothing.Path);
        PlayerPrefs.SetString(SaveKeys.EquippedItemPath, player.equippedSpecialItem.Path);

        List<string> InventoryItemsNames = player.Inventory.Select(item => item.Path).ToList();
        string itemJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = InventoryItemsNames });
        PlayerPrefs.SetString(SaveKeys.InventoryItemsPaths, itemJson);

        List<string> AbilityNames = player.abilities.Select(ability => ability.Path).ToList();
        string abilitiesJson = JsonUtility.ToJson(new StringListWrapper { savedStrings = AbilityNames });
        PlayerPrefs.SetString(SaveKeys.AbilitiesPaths, abilitiesJson);

        PlayerPrefs.Save();
    }

    [Serializable]
    public class StringListWrapper
    {
        public List<string> savedStrings;
    }

    public static class SaveKeys
    {
        public const string Scene = "Scene";
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

    public void LoadGame()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // if loading game need to wait for scene to be loaded then destroy iteself.
        DontDestroyOnLoad(this.gameObject);

        // load scene
        var scenePlayerSavedIn = PlayerPrefs.GetString(SaveKeys.Scene);
        TransitionManager.Instance().Transition(scenePlayerSavedIn, transition, 0f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // prevent multiple calls

        PlayerDungeon player = FindObjectOfType<PlayerDungeon>();
        if (player == null)
        {
            Debug.LogError("Player not found in loaded scene.");
            return;
        }

        LoadSaveData(player);

        // then destroy this gameobject no longer needed in scene
        Destroy(this.gameObject);
    }

    private void LoadSaveData(PlayerDungeon player)
    {
        // deserialize saved items.
        string itemsJson = PlayerPrefs.GetString(SaveKeys.InventoryItemsPaths);
        player.Inventory = DeserializeSavedStrings<Item>(itemsJson);

        // deserialize saved abilities
        string abilitiesJson = PlayerPrefs.GetString(SaveKeys.AbilitiesPaths);
        player.abilities = DeserializeSavedStrings<Ability>(abilitiesJson);

        // the other stuff except scene already done
        player.gameObject.transform.position = new Vector3(
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosX),
            PlayerPrefs.GetFloat(SaveKeys.PlayerPosY),
            player.gameObject.transform.position.z);

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

    private List<T> DeserializeSavedStrings<T>(string json) where T : ScriptableObject
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

    public void QuitGame()
    {

    }
}