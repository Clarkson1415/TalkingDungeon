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
                    SetupButtons();
                    StartCoroutine(WriteSlideOverTime());
                    State = BoxState.writing;
                }
                break;
            case BoxState.writing:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    this.SkipToEnd();
                    this.State = BoxState.waitingOnSlide;
                }
                if (this.FinishedWritingSlideOverTime)
                {
                    this.FinishedWritingSlideOverTime = false;
                    this.State = BoxState.waitingOnSlide;
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
                        this.State = BoxState.invisibleInactive;
                    }
                    else
                    {
                        if (this.currentSlide.options != null)
                        {
                            var selected = this.currentSlide.options.First(x => x.isSelected);
                            this.currentSlide = selected.nextDialogueSlide;
                        }
                        else
                        {
                            this.currentSlide = this.currentSlide.nextSlide;
                        }

                        SetupButtons();
                        StartCoroutine(WriteSlideOverTime());
                        this.State = BoxState.writing;
                    }
                }
                break;
            default:
                State = BoxState.invisibleInactive;
                break;
        }
    }

    public void SkipToEnd()
    {
        Debug.Log("skip to end");
        StopAllCoroutines();
        this.TMPTextBox.text = this.currentSlide.dialogue;
    }

    private void SetupButtons()
    {
        this.buttons.Clear();

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
                this.buttons.Add(buttonGameObj);
            }

            // UIEventSystem.firstSelectedGameObject = this.buttons[0];
        }
    }

    IEnumerator WriteSlideOverTime()
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
        if (this.buttons.Count > 0)
        {
            foreach (var button in this.buttons)
            {
                button.GetComponentInChildren<TMP_Text>().text = button.GetComponent<DialogueOption>().optionText;
            }
        }

        this.FinishedWritingSlideOverTime = true;
    }
}
