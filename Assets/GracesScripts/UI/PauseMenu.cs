using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menu;
    [SerializeField] private List<GameObject> menuButtons;
    private GameObject currentHighlighted;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartPauseMenu()
    {
        this.UIEventSystem.SetSelectedGameObject(menuButtons[0]);
        currentHighlighted = menuButtons[0];
    }

    private void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        if (highlightedMenuItem != currentHighlighted && currentHighlighted != null)
        {
            if (highlightedMenuItem.TryGetComponent<MenuButton>(out var button))
            {
                button.PlayHighlightOptionChangedSound();
                currentHighlighted = this.UIEventSystem.currentSelectedGameObject;
            }
        }
    }
}
