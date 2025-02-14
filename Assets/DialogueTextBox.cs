using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
#nullable enable

public class DialogueTextBox : MonoBehaviour
{
    private IHasDialogue? NPCPlayerIsSpeakingTo;
    private TMP_Text TMPTextBox;
    private string? currentTextDialogue;
    [SerializeField] private float textspeed = 0.1f;
    public bool HasShownLastSlide;
    private DialogueBoxState state = DialogueBoxState.invisibleInactive;

    private enum DialogueBoxState
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
        //switch (state)
        //{
        //    case DialogueBoxState.invisibleInactive:
        //        break;
        //    case DialogueBoxState.writing:

        //}

        

        // if knight.interact fired:
        // update to new speaker
        // state = writing.

    }

    public void OnNewSpeaker(IHasDialogue newSpeaker)
    {
        this.NPCPlayerIsSpeakingTo = newSpeaker;
        StartCoroutine(BeginNewDialogue(newSpeaker.GetDialogue(), newSpeaker.GetDialogueOptions()));
    }

    public void OnConversationFinished()
    {
        this.NPCPlayerIsSpeakingTo = null;
    }

    public void SkipToEnd()
    {
        // TODO: not string here ew
        Debug.Log("skip to end");
        StopAllCoroutines();
        this.TMPTextBox.text = this.currentTextDialogue;

        this.HasShownLastSlide = true;
    }

    IEnumerator BeginNewDialogue(string[] dialogueSlides, string[][] options)
    {
        string phrase = "many many test words wow you have a lot to say girl damn.";
        currentTextDialogue = phrase;

        for (int i = 0; i < phrase.Length ; i++)
        {
            if (i == 0)
            {
                this.TMPTextBox.SetText(phrase[0].ToString());
                continue;
            }

            this.TMPTextBox.text += phrase[i];
            yield return new WaitForSeconds(textspeed);
        }

        this.HasShownLastSlide = true;

        //bool hasOptions = false;

        //// test single first slide.
        //int i = 0;
        //if (options[i].Length > 0)
        //{
        //    hasOptions = true;
        //}

        //foreach (var letter in dialogueSlides[i])
        //{
        //    this.TMPTextBox.text += letter;
        //    yield return new WaitForSeconds(0.5f);
        //}

        //if (hasOptions)
        //{
        //    this.TMPTextBox.text += "\n";

        //    foreach (var option in options[i])
        //    {
        //        this.TMPTextBox.text += option;
        //    }
        //}
    }
}
