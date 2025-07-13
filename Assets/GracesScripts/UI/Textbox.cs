using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

/// <summary>
/// Represents a class that prints text.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public abstract class Textbox : MenuWithButtons
{
    public bool IsNotWritingOrOnSlide => this.State == BoxState.INACTIVE;

    protected TMP_Text TMPTextBox;
    protected AudioSource dialoguePrintAudio;

    protected BoxState State { get; set; } = BoxState.INACTIVE;
    [SerializeField] protected float textspeed = 0.1f;
    [SerializeField] protected float underscorePauseTime = 0.01f;
    protected const char pauseCharacterToNotPrint = '_';
    protected Coroutine? writingSlide;
    protected bool finishedWritingSlide;
    protected bool InteractFlag;
    protected bool ButtonClickedFlagSet;
    private string textBeingWritten;

    private void Awake()
    {
        TMPTextBox = this.GetComponentInChildren<TMP_Text>();
        this.dialoguePrintAudio = GetComponent<AudioSource>();
        this.dialoguePrintAudio.loop = false;
    }

    public void StartWriting(string text, Color colour)
    {
        this.textBeingWritten = text;
        this.State = BoxState.WRITINGSLIDE;
        writingSlide = this.StartCoroutine(this.WriteSlideOverTime(text, colour));
    }

    protected virtual void ResetAllFlags()
    {
        InteractFlag = false;
        ButtonClickedFlagSet = false;
    }

    protected enum BoxState
    {
        INACTIVE,
        WRITINGSLIDE,
        WAITINGONSLIDE,
    }

    protected void SkipToEnd()
    {
        // In more recent versions of Unity (at least 5.3 onwards) you can keep a reference to the IEnumerator or returned Coroutine object and start and stop that directly, rather than use the method name. These are preferred over using the method name as they are type safe and more performant. See the StopCoroutine docs for details https://docs.unity3d.com/ScriptReference/MonoBehaviour.StopCoroutine.html
        string stringWithoutUnderscores = "";
        foreach (var item in textBeingWritten)
        {
            if (item != pauseCharacterToNotPrint)
            {
                stringWithoutUnderscores += item;
            }
        }

        if (writingSlide != null)
        {
            StopCoroutine(writingSlide);
        }

        this.TMPTextBox.text = stringWithoutUnderscores;
        this.finishedWritingSlide = true;
    }

    protected IEnumerator WriteSlideOverTime(string text, Color color)
    {
        this.finishedWritingSlide = false;

        // get the autosized font size. then reprint the text at that size without autosize enabled so it doesnt change size while printing.
        this.TMPTextBox.enableAutoSizing = true;
        this.TMPTextBox.text = text;
        this.TMPTextBox.ForceMeshUpdate();
        var fontSize = this.TMPTextBox.fontSize;
        this.TMPTextBox.text = string.Empty;
        this.TMPTextBox.enableAutoSizing = false;
        this.TMPTextBox.fontSize = fontSize;
        this.TMPTextBox.ForceMeshUpdate();
        this.TMPTextBox.color = color;

        for (int i = 0; i < text.Length; i++)
        {
            // don't play sound for either the special pause text printing character, or spaces. 
            if (text[i] == pauseCharacterToNotPrint)
            {
                yield return new WaitForSeconds(underscorePauseTime);
                continue;
            }

            // dont play a sound but do print a space empty char
            if (text[i] == ' ')
            {
                this.TMPTextBox.text += text[i];
                yield return new WaitForSeconds(textspeed);
                continue;
            }

            this.PlayDialoguePrintAudio();
            if (i == 0) // set first letter if this is the first letter.
            {
                this.TMPTextBox.SetText(text[0].ToString());
                continue;
            }

            // do the rest of the letters
            this.TMPTextBox.text += text[i];
            yield return new WaitForSeconds(textspeed);
        }

        this.finishedWritingSlide = true;
        writingSlide = null;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        Log.Print("Interact flag set");

        this.InteractFlag = true;
    }

    private void PlayDialoguePrintAudio()
    {
        this.dialoguePrintAudio.Play();
    }
}