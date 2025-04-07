using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistanctSaveData : MonoBehaviour
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

    private void Awake()
    {
        // on awake see if there is another save data in the scene
        // if there is than this one called awake and is the duplicate set in the future scene that is now loaded
        // and destroy this object. and the save data from previous level should be used.
        // all levels have one of these so that I can playtest from each level.
        var saveDataObjects = FindObjectsByType<PersistanctSaveData>(FindObjectsSortMode.None);
        if(saveDataObjects.Count() > 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SavePlayerData()
    {
        var player = FindObjectOfType<PlayerDungeon>();

        this.currentWellbeing = player.currentWellbeing;
        this.maxWellbeing = player.maxWellbeing;
        this.abilities = player.abilities;
        this.power = player.power;
        this.defence = player.defence;
        this.Inventory = player.Inventory;
        this.equippedClothing = player.equippedClothing;
        this.equippedSpecialItem = player.equippedSpecialItem;
        this.equippedWeapon = player.equippedWeapon;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
