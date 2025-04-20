using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] EventSystem UIEventSystem;
    [SerializeField] GameObject LoadLastSaveButton;
    [SerializeField] GameObject StartNewGameButton;
    [SerializeField] GameObject startLoadButtonLocation;
    public bool PretendFirstTimeBootDeleteSave = false;
    public string sceneToLoad = TalkingDungeonScenes.Intro;

    // Start is called before the first frame update
    void Start()
    {
        if (PretendFirstTimeBootDeleteSave)
        {
            Debug.LogWarning("Deleting all player prefs...");
            PlayerPrefs.DeleteAll();
        }

        var savedScene = PlayerPrefs.GetString(PlayerDataUtility.SaveKeys.LastScene);
        if (string.IsNullOrEmpty(savedScene))
        {
            var topButton = Instantiate(StartNewGameButton, startLoadButtonLocation.transform);
            this.UIEventSystem.SetSelectedGameObject(topButton);
            topButton.GetComponent<MenuButton>().sceneToLoad = TalkingDungeonScenes.Intro;
        }
        else
        {
            var topButton = Instantiate(LoadLastSaveButton, startLoadButtonLocation.transform);
            this.UIEventSystem.SetSelectedGameObject(topButton);
        }
    }

    private GameObject currentlyShownItem;

    // Update is called once per frame
    void Update()
    {
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        // on menu open after another has been open do onece
        if (highlightedMenuItem == null)
        {
            return;
        }

        if (currentlyShownItem == null)
        {
            currentlyShownItem = highlightedMenuItem;
        }

        // when it is open do this
        if (highlightedMenuItem != currentlyShownItem && currentlyShownItem != null)
        {
            currentlyShownItem = highlightedMenuItem;

            if (highlightedMenuItem.TryGetComponent<MenuButton>(out var button))
            {
                button.PlayHighlightOptionChangedSound();
            }
        }
    }
}
