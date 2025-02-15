using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
#nullable enable

public class DialogueTextBox : MonoBehaviour
{
    private IHasDialogue? NPCPlayerIsSpeakingTo;
    private TMP_Text TMPTextBox;
    [SerializeField] private float textspeed = 0.1f;
    private DialogueSlide? currentSlide;
    public BoxState State { get; private set; } = BoxState.invisibleInactive;
    public bool PlayerInteractFlagSet;
    [SerializeField] GameObject prefabButton;
    [SerializeField] EventSystem UIEventSystem;
    List<GameObject> buttons = new List<GameObject>();
    [SerializeField] float buttonSpacing = 170;
    [SerializeField] private GameObject firstButtonLocationObject;

    public void NewInteractionBegan(DialogueSlide firstSlide)
    {
        currentSlide = firstSlide;
        this.newInteractionSetup = true;
    }

    private bool newInteractionSetup;

    public enum BoxState
    {
        invisibleInactive,
        writing,
        waitingOnSlide,
    }

    private void Awake()
    {
        TMPTextBox = this.GetComponentInChildren<TMP_Text>();
    }

    private bool FinishedWritingSlideOverTime;

    private void Update()
    {
        switch (State)
        {
            case BoxState.invisibleInactive:
                if (this.PlayerInteractFlagSet && this.newInteractionSetup)
                {
                    this.newInteractionSetup = false;
                    this.PlayerInteractFlagSet = false;
                    StartCoroutine(WriteSlideOverTime());
                    this.FinishedWritingSlideOverTime = false;
                    State = BoxState.writing;
                    Debug.Log("state writing");
                }
                break;
            case BoxState.writing:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    this.SkipToEnd();
                    this.State = BoxState.waitingOnSlide;
                    Debug.Log("state writing");
                }
                if (this.FinishedWritingSlideOverTime)
                {
                    this.FinishedWritingSlideOverTime = false;
                    this.State = BoxState.waitingOnSlide;
                    Debug.Log("state waitingOnSlide");
                }
                break;
            case BoxState.waitingOnSlide:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    if (this.currentSlide.lastSlideInSequence)
                    {
                        this.currentSlide = null;
                        this.gameObject.SetActive(false);
                        Debug.Log("state INVIS INACTIVE");
                        this.State = BoxState.invisibleInactive;
                    }
                    else
                    {
                        if (this.currentSlide.options == null || this.currentSlide.options.Count == 0)
                        {
                            this.currentSlide = this.currentSlide.nextSlide;
                        }
                        else
                        {
                            var selected = this.buttons.First(x => x.GetComponent<DialogueOption>().isSelected);
                            this.currentSlide = selected.GetComponent<DialogueOption>().nextDialogueSlide;
                        }

                        StartCoroutine(WriteSlideOverTime());
                        Debug.Log("state writing after waiting");
                        this.State = BoxState.writing;
                    }
                }
                break;
            default:
                Debug.Log("default set invis inactive");
                State = BoxState.invisibleInactive;
                break;
        }
    }

    public void SkipToEnd()
    {
        Debug.Log("skip to end");
        StopAllCoroutines();
        DrawButtons();
        this.TMPTextBox.text = this.currentSlide.dialogue;
    }

    private void DrawButtons()
    {
        if(this.currentSlide?.options == null)
        {
            return;
        }

        if (this.currentSlide.options.Count > 0)
        {
            Vector3 positionVector = new Vector3(0, 0, 0);

            for (int i = 0; i < this.currentSlide.options.Count; i++)
            {
                // Instantiate new button with that gameobject as parent.
                var buttonGameObj = Instantiate(this.prefabButton, this.firstButtonLocationObject.transform);
                // calculate positon offset. 
                positionVector = this.firstButtonLocationObject.transform.position;
                positionVector.y -= buttonSpacing * i;
                // set position of button correctly.
                buttonGameObj.transform.SetPositionAndRotation(positionVector, Quaternion.identity);

                // set button Dialogue Option to the Dialogue Option.
                // REMEBER THIS IS NOT THE SAME OBJECT AS IN THE CURRENT SLIDE.OPTIONS
                buttonGameObj.GetComponent<DialogueOption>().SetValues(this.currentSlide.options[i]);

                // TODO: not sure if this will add the correct button to the list?
                this.buttons.Add(buttonGameObj);
            }

            this.UIEventSystem.SetSelectedGameObject(this.buttons[0]);
        }

        // draw options
        if (this.buttons.Count > 0)
        {
            foreach (var button in this.buttons)
            {
                button.GetComponentInChildren<TMP_Text>().text = button.GetComponent<DialogueOption>().optionText;
            }
        }
    }

    IEnumerator WriteSlideOverTime()
    {
        // remove old buttons 
        this.buttons.ForEach(x => Destroy(x));
        this.buttons.Clear();

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

        DrawButtons();

        this.FinishedWritingSlideOverTime = true;
    }
}
