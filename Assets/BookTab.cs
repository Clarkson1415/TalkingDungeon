using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To switch tab layout to tab selected. And pull the logo out into the correct position.
/// </summary>
public class BookTab : DungeonButton
{
    [SerializeField] public BookTabCategory tabCategory;

    [SerializeField] Sprite uiUnselectedSprite;
    [SerializeField] Sprite TabSelectedSprite;
    [SerializeField] Image imageToSwitchSelectedTabSprite;

    /// <summary>
    /// used to keep the tab sprite the same. cannot use unity button selected as that changes when anotehr thing is selected in the menu 
    /// </summary>
    /// <param name="isSelected"></param>
    public void SwapTabSprite(bool isSelected)
    {
        if (isSelected)
        {
            this.imageToSwitchSelectedTabSprite.sprite = TabSelectedSprite;
        }
        else
        {
            this.imageToSwitchSelectedTabSprite.sprite = uiUnselectedSprite;
        }
    }
}
