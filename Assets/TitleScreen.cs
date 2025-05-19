using Assets.GracesScripts.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreen : MenuWithButtons
{
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
        
        GameObject topButton;

        var savedScene = PlayerPrefs.GetString(PlayerDataUtility.SaveKeys.LastScene);
        if (string.IsNullOrEmpty(savedScene))
        {
            topButton = Instantiate(StartNewGameButton, startLoadButtonLocation.transform);
            topButton.GetComponent<MenuButton>().sceneToLoad = TalkingDungeonScenes.Intro;
        }
        else
        {
            topButton = Instantiate(LoadLastSaveButton, startLoadButtonLocation.transform);
        }
    }

    //// this Update is only needed if player uses keyabord wasd to navigate buttons
    //private void Update()
    //{
    //    var highlightedButton = this.UIEventSystem.currentSelectedGameObject;

    //    if (highlightedButton == lastHighlightedItem)
    //    {
    //        return;
    //    }

    //    if (highlightedButton == null)
    //    {
    //        return;
    //    }

    //    lastHighlightedItem = highlightedButton;

    //    var button = highlightedButton.GetComponent<DungeonButton>();
    //    button.PlayHighlightedSound();
    //}
}
