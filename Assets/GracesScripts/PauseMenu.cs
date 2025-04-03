using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] EventSystem UIEventSystem;
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
            if (highlightedMenuItem.TryGetComponent<ButtonMenuOption>(out var button))
            {
                button.PlayHighlightOptionChangedSound();
                currentHighlighted = this.UIEventSystem.currentSelectedGameObject;
            }
        }
    }

    /// <summary>
    /// TODO: really slow too many get components. ALSO im not even using the ItemOptnButton.ClickButton() event setup in the prefab to trigger on click. but that could be where the sound is played instead 
    /// </summary>
    public GameObject GetSelectedButton()
    {
        return this.UIEventSystem.currentSelectedGameObject;
    }
}
