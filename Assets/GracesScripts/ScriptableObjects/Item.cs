using Assets.GracesScripts;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/item", order = 1)]
public class Item : ScriptableObject
{
    public int PowerStat;
    public int DefenceStat;

    public ItemType Type;
    public string Name;
    public string description;

    public string Path => $"Items/{this.Type}/{this.Name}";

    /// <summary>
    /// todo: resize to fit the menu screen of objects and do this for profile pic too
    /// </summary>
    public Sprite image;
}