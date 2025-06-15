using Assets.GracesScripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

public class PlayerDungeonData
{
    public int CurrentHealth;
    public int MaxHealth;
    public Weapon EquippedWeapon;
    public SpecialItem? EquippedItem;
    public Vector3 Position;
    public List<DungeonItem> Inventory;
    public PlayerDungeonData LastSavedData;
}