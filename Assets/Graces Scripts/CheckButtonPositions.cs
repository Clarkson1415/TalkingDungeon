using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckButtonPositions : MonoBehaviour
{
    private DialogueSlide currentSlide;
    [SerializeField] GameObject prefabButton;
    private List<GameObject> buttons_or_CurrentDialogueOptions = new List<GameObject>();
    [SerializeField] float buttonSpacing = 170f;
    [SerializeField] private EventSystem UIEventSystem;
    [SerializeField] TMP_Text TMPTextBox;
    [SerializeField] float textspeed = 0.05f;
    [SerializeField] private GameObject firstButtonLocationObject;

    // Start is called before the first frame update
    private void Start()
    {
        DialogueSlide Slide2_lastSlide = new DialogueSlide("k bye", true);
        DialogueOption Slide1Option1 = new DialogueOption("what?", Slide2_lastSlide);
        DialogueOption Slide1Option2 = new DialogueOption("Fuck you", Slide2_lastSlide);
        DialogueSlide slide1 = new DialogueSlide("HI", new List<DialogueOption>()
        { Slide1Option1, Slide1Option2});

        currentSlide = slide1;
        SetupButtons();
        StartCoroutine(WriteSlideOverTimeCheck());
    }

    private void SetupButtons()
    {
        this.buttons_or_CurrentDialogueOptions.Clear();

        if (this.currentSlide.options != null || this.currentSlide.options.Count > 0)
        {
            Vector3 positionVector = new Vector3(0, 0, 0);

            for (int i = 0; i < this.currentSlide.options.Count; i++)
            {
                // Instantiate new button with that gameobject as parent.
                var buttonGameObj = Instantiate(this.prefabButton, this.firstButtonLocationObject.transform);
                // calculate positon offset. 
                positionVector = this.firstButtonLocationObject.transform.position;
                positionVector.x += buttonSpacing * i;
                // set position of button correctly.
                buttonGameObj.transform.SetPositionAndRotation(positionVector, Quaternion.identity);

                // set button Dialogue Option to the Dialogue Option.
                // REMEBER THIS IS NOT THE SAME OBJECT AS IN THE CURRENT SLIDE.OPTIONS
                buttonGameObj.GetComponent<DialogueOption>().SetValues(this.currentSlide.options[i]);

                // TODO: not sure if this will add the correct button to the list?
                this.buttons_or_CurrentDialogueOptions.Add(buttonGameObj);
            }

            // TODO dont forget to add text
            // trasform is recttransform
            UIEventSystem.firstSelectedGameObject = this.buttons_or_CurrentDialogueOptions[0];
        }
    }

    IEnumerator WriteSlideOverTimeCheck()
    {
        for (int i = 0; i < this.currentSlide.dialogue.Length; i++)
        {
            if (i == 0)
            {
                this.TMPTextBox.SetText(this.currentSlide.dialogue[0].ToString());
                continue;
            }

            this.TMPTextBox.text += this.currentSlide.dialogue[i];
            yield return new WaitForSeconds(textspeed);
        }

        // draw options
        if (this.buttons_or_CurrentDialogueOptions.Count > 0)
        {
            foreach (var button in this.buttons_or_CurrentDialogueOptions)
            {
                button.GetComponentInChildren<TMP_Text>().text = button.GetComponent<DialogueOption>().optionText;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
