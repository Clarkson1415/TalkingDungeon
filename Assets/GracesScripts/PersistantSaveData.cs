using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PersistantSaveData : MonoBehaviour
{
    public float currentWellbeing = 100;
    public float maxWellbeing = 100;
    public List<Ability> abilities = new();
    public int power = 0;
    public int defence = 0;
    public List<Item> Inventory;
    public Item? equippedWeapon;
    public Item? equippedClothing;
    public Item? equippedSpecialItem;

    /// <summary>
    /// Keeps track of what scenes have been loaded this playthough
    /// </summary>
    public int scenesGoneThrough = 0;

    public void SavePlayerData()
    {
        var player = FindObjectOfType<PlayerDungeon>();

        this.currentWellbeing = player.currentWellbeing;
        this.maxWellbeing = player.maxWellbeing;
        this.abilities = player.abilities;
        this.Inventory = player.Inventory;
        this.equippedClothing = player.equippedClothing;
        this.equippedSpecialItem = player.equippedSpecialItem;
        this.equippedWeapon = player.equippedWeapon;
    }
}
