using UnityEngine;

/// <summary>
/// To switch tab layout to tab selected. And pull the logo out into the correct position.
/// </summary>
public class BookTab : MonoBehaviour
{
    [SerializeField] private BookTabCategory tabCategory;

    private enum BookTabCategory
    {
        Items,
        Weapons,
        Clothing,
        Abilities,
        // DialogueLog
    }

    // starting logo position (-4.4, 0)
    // when clicked on move to 6.1, 0 
    // add 10.5 to the x position.

    public void NavigateToSelectedTabCategory()
    {
        Debug.Log($"to tab: {this.tabCategory}");
    }
}
