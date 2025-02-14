using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
#nullable enable

public class DialogueTextBox : MonoBehaviour
{
    private IHasDialogue? NPCPlayerIsSpeakingTo;
    private TMP_Text TMPTextBox;
    [SerializeField] private float textspeed = 0.1f;
    private DialogueSlide? currentSlide;
    public BoxState State { get; private set; } = BoxState.invisibleInactive;
    public bool PlayerInteractFlagSet;

    public void NewInteractionBegan(DialogueSlide firstSlide)
    {
        currentSlide = firstSlide;
    }

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

    private void Update()
    {
        switch (State)
        {
            case BoxState.invisibleInactive:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
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
                break;
            case BoxState.waitingOnSlide:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    if (this.currentSlide.lastSlideInSequence)
                    {
                        this.currentSlide = null;
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

    IEnumerator WriteSlideOverTime()
    {
        for (int i = 0; i < this.currentSlide.dialogue.Length ; i++)
        {
            if (i == 0)
            {
                this.TMPTextBox.SetText(this.currentSlide.dialogue[0].ToString());
                continue;
            }

            this.TMPTextBox.text += this.currentSlide.dialogue[i];
            yield return new WaitForSeconds(textspeed);
        }

        if (this.currentSlide.options != null || this.currentSlide.options?.Count == 0)
        {
            this.TMPTextBox.text += "\n";

            foreach (var option in this.currentSlide.options)
            {
                foreach(var character in option.optionText)
                {
                    this.TMPTextBox.text += character;
                }

                this.TMPTextBox.text += "\n";
            }
        }
    }
}
