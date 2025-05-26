using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonItem : ScriptableObject
{
    public string Name;
    public string description;
    public abstract string Path { get; set; }

    /// <summary>
    /// todo: resize to fit the menu screen of objects and do this for profile pic too
    /// </summary>
    public Sprite image;
}