using Assets.GracesScripts;
using UnityEngine;

/// <summary>
/// To switch tab layout to tab selected. And pull the logo out into the correct position.
/// </summary>
public class BookTab : DungeonButton
{
    public Sprite HighlightedSprite;
    public Sprite SelectedSprite;
    public Sprite PressedSprite;

    public enum TabType
    {
        Gear,
        Items,
        Save,
        Settings,
    }

    public TabType tabType;
}
