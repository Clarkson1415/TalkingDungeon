using Assets.GracesScripts.UI;
using UnityEngine;

public class TitleScreen : MenuWithButtons
{
    [SerializeField] GameObject LoadSaveButton;
    [SerializeField] GameObject ContinueButtonLocation;
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

        var savedScene = PlayerPrefs.GetString(SaveGameUtility.SaveKeys.LastScene);
        if (!string.IsNullOrEmpty(savedScene))
        {
            var topButton = Instantiate(LoadSaveButton, ContinueButtonLocation.transform);
            topButton.gameObject.transform.SetAsFirstSibling();
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
