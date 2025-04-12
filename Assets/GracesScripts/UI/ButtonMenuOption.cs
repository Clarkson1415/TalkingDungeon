using Assets.GracesScripts;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMenuOption : DungeonButton
{
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


    }

    public static class SaveKeys
    {
        public const string Scene = "Scene";
        public const string PlayerPosX = "PlayerPosX";
        public const string PlayerPosY = "PlayerPosY";
        public const string playerCurrentWellbeing = "CurrentWellbeing";
        public const string PlayerMaxWellbeing = "MaxWellbeing";
        public const string PlayerInventory = "Inventory";
        public const string PlayerEquippedWeapon = "EquippedWeapon";
        public const string PlayerEquippedClothing = "EquippedClothing";
        public const string PlayerEquippedItem = "EquippedItem";


        // TODO Dialogue log should be saved
        //public const string DialogueLog = "Dialogue";
    }

    public void LoadGame()
    {

    }

    public void QuitGame()
    {

    }
}