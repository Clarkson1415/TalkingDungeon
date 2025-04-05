using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/item", order = 1)]
public class Item : ScriptableObject
{
    public int PowerStat;
    public int DefenceStat;

    public ItemType Type;
    public string Name;
    public string description;

    /// <summary>
    /// todo: resize to fit the menu screen of objects and do this for profile pic too
    /// </summary>
    public Sprite image;
}