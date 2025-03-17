using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#nullable enable

public class DialogueTextBox : MonoBehaviour
{
    private Coroutine? writeSlidesOverTimeCoroutine = null;
    private bool newInteractionSetup;
    private bool FinishedWritingSlideOverTime;
    private const char pauseCharacterToNotPrint = '_';
    private TMP_Text TMPTextBox;
    private DialogueSlide? currentSlide;

    public BoxState State { get; private set; } = BoxState.WAITINGFORINTERACTION;
    public bool PlayerInteractFlagSet;
    readonly List<GameObject> buttons = new();
    [SerializeField] private float textspeed = 0.1f;
    [SerializeField] private float underscorePauseTime = 0.01f;
    [SerializeField] GameObject prefabButton;
    [SerializeField] EventSystem UIEventSystem;
    [SerializeField] AudioSource dialogueSoundEffectAudioSource;
    [SerializeField] List<GameObject> buttonPositionsTopToBottom;
    [SerializeField] Image speakersPicRenderer;

    public void BeginDialogue(DialogueSlide firstSlide)
    {
        this.newInteractionSetup = true;
        this.UpdateCurrentSlide(firstSlide);
    }

    public enum BoxState
    {
        WAITINGFORINTERACTION,
        WRITINGSLIDE,
        WAITINGONSLIDE,
    }

    private void Awake()
    {
        TMPTextBox = this.GetComponentInChildren<TMP_Text>();
        this.dialogueSoundEffectAudioSource.loop = false;
    }

    private void UpdateCurrentSlide(DialogueSlide? newSlide)
    {
        if (newSlide == null)
        {
            this.currentSlide = null;
            this.speakersPicRenderer.sprite = null;
        }
        else
        {
            this.currentSlide = newSlide;
            this.speakersPicRenderer.sprite = newSlide.SpeakerPic;
        }
    }

    private void Update()
    {
        switch (State)
        {
            case BoxState.WAITINGFORINTERACTION:
                if (this.PlayerInteractFlagSet && this.newInteractionSetup)
                {
                    this.newInteractionSetup = false;
                    this.PlayerInteractFlagSet = false;
                    this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                    this.FinishedWritingSlideOverTime = false;
                    State = BoxState.WRITINGSLIDE;
                    //Log.Print("writing Slide");
                }
                break;
            case BoxState.WRITINGSLIDE:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    this.SkipToEnd();
                    this.State = BoxState.WAITINGONSLIDE;
                    //Log.Print("state writing");
                }
                if (this.FinishedWritingSlideOverTime)
                {
                    this.FinishedWritingSlideOverTime = false;
                    this.State = BoxState.WAITINGONSLIDE;
                    //Log.Print("state waitingOnSlide");
                }
                break;
            case BoxState.WAITINGONSLIDE:
                if (this.PlayerInteractFlagSet)
                {
                    this.PlayerInteractFlagSet = false;
                    MyGuard.IsNotNull(this.currentSlide);
                    if (this.currentSlide.islastSlideInSequence)
                    {
                        UpdateCurrentSlide(null);
                        //Log.Print("state INVIS INACTIVE");
                        this.speakersPicRenderer.sprite = null;
                        this.State = BoxState.WAITINGFORINTERACTION;
                        Debug.Log("Box in Set to waiting for interaction");
                        this.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (this.currentSlide.options == null || this.currentSlide.options.Count == 0)
                        {
                            MyGuard.IsNotNull(this.currentSlide.nextSlide);
                            this.UpdateCurrentSlide(this.currentSlide.nextSlide);
                        }
                        else
                        {
                            var selectedDiaOption = this.buttons.First(x => x.GetComponent<DialogueOptionButton>().isSelected);
                            if (selectedDiaOption.GetComponent<DialogueOptionButton>().NextDialogueSlide == null)
                            {
                                Log.Print("next dialogue slide null did you mean to finish conversation?");
                            }

                            this.UpdateCurrentSlide(selectedDiaOption.GetComponent<DialogueOptionButton>().NextDialogueSlide);
                        }

                        this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                        //Log.Print("state writing after waiting");
                        this.State = BoxState.WRITINGSLIDE;
                    }
                }
                break;
            default:
                State = BoxState.WAITINGFORINTERACTION;
                break;
        }
    }

    private void SkipToEnd()
    {
        // In more recent versions of Unity (at least 5.3 onwards) you can keep a reference to the IEnumerator or returned Coroutine object and start and stop that directly, rather than use the method name. These are preferred over using the method name as they are type safe and more performant. See the StopCoroutine docs for details https://docs.unity3d.com/ScriptReference/MonoBehaviour.StopCoroutine.html
        StopCoroutine(writeSlidesOverTimeCoroutine);
        DrawButtons();
        string parsedString = "";
        MyGuard.IsNotNull(currentSlide);
        MyGuard.IsNotNull(currentSlide.dialogue);
        foreach (var item in currentSlide.dialogue)
        {
            if (item != pauseCharacterToNotPrint)
            {
                parsedString += item;
            }
        }

        this.TMPTextBox.text = parsedString;
    }

    private void DrawButtons()
    {
        if (this.currentSlide?.options == null)
        {
            return;
        }

        if (this.currentSlide.options.Count > 0)
        {
            Vector3 positionVector = new Vector3(0, 0, 0);

            for (int i = 0; i < this.currentSlide.options.Count; i++)
            {
                // Instantiate new button with that gameobject as parent.
                var buttonGameObj = Instantiate(this.prefabButton, this.buttonPositionsTopToBottom[i].transform);

                // set button Dialogue Option to the Dialogue Option.
                // REMEBER THIS IS NOT THE SAME OBJECT AS IN THE CURRENT SLIDE.OPTIONS
                buttonGameObj.GetComponent<DialogueOptionButton>().SetValues(this.currentSlide.options[i]);
                this.buttons.Add(buttonGameObj);
            }

            this.UIEventSystem.SetSelectedGameObject(this.buttons[0]);
        }

        // draw options
        if (this.buttons.Count > 0)
        {
            foreach (var button in this.buttons)
            {
                button.GetComponentInChildren<TMP_Text>().text = button.GetComponent<DialogueOptionButton>().OptionText;
            }
        }
    }

    private IEnumerator WriteSlideOverTime()
    {
        // remove old buttons 
        this.buttons.ForEach(x => Destroy(x));
        this.buttons.Clear();

        MyGuard.IsNotNull(this.currentSlide);
        MyGuard.IsNotNull(this.currentSlide.dialogue);
        for (int i = 0; i < this.currentSlide.dialogue.Length; i++)
        {
            // don't play sound for either the special pause text printing character, or spaces. 
            if (this.currentSlide.dialogue[i] == pauseCharacterToNotPrint)
            {
                yield return new WaitForSeconds(underscorePauseTime);
                continue;
            }

            // dont play a sound but do print a space empty char
            if (this.currentSlide.dialogue[i] == ' ')
            {
                this.TMPTextBox.text += this.currentSlide.dialogue[i];
                yield return new WaitForSeconds(textspeed);
                continue;
            }

            // play dialogue text sound effect
            this.dialogueSoundEffectAudioSource.Play();

            if (i == 0) // set first letter if this is the first letter.
            {
                this.TMPTextBox.SetText(this.currentSlide.dialogue[0].ToString());
                continue;
            }

            // do the rest of the letters
            this.TMPTextBox.text += this.currentSlide.dialogue[i];
            yield return new WaitForSeconds(textspeed);
        }

        DrawButtons();
        this.FinishedWritingSlideOverTime = true;
    }
}
