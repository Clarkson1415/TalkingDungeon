using Assets.GracesScripts;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// To switch tab layout to tab selected. And pull the logo out into the correct position.
/// </summary>
public class BookTab : DungeonButton
{
    public ItemType Category;
    private Animator tabAnimator;

    private void Awake()
    {
        tabAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// used to keep the tab sprite the same. cannot use unity button selected as that changes when anotehr thing is selected in the menu 
    /// </summary>
    /// <param name="forceSelected"></param>
    public void ForceTabSelectionAnim(bool isSelected)
    {
        if (isSelected)
        {
            tabAnimator.SetTrigger("Selected");
        }
        else // force unselected
        {
            tabAnimator.SetTrigger("ManualForceNormal");
        }
    }
}
