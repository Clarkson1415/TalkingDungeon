using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
#nullable enable

[RequireComponent(typeof(AudioSource))]
public class DialogueTextBox : MenuWithButtons
{
    [Header("Battle Stuff")]
    public TransitionSettings transitionForGoingToBattleScene;
    public DialogueSlide? CurrentSlide { get; private set; }
    public BoxState State { get; private set; } = BoxState.WAITINGFORINTERACTION;
    [SerializeField] private float textspeed = 0.1f;
    [SerializeField] private float underscorePauseTime = 0.01f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip dialoguePrintingAudio;
    [SerializeField] private List<DialogueOptionButton> buttons;
    private Coroutine? writeSlidesOverTimeCoroutine = null;
    private bool FinishedWritingSlideOverTime;
    private const char pauseCharacterToNotPrint = '_';
    private TMP_Text TMPTextBox;
    public TMP_Text speakerNameText;
    private Unit_NPC currentSpeaker;

    /// <summary>
    /// for the player statemachine to recognise the interaction has finished.
    /// </summary>
    [HideInInspector] public bool finishedInteractionFlag;
    [HideInInspector] public bool PlayerInteractFlagSet;
    [HideInInspector] public bool ButtonClickedFlagSet;
    private bool newDialogueStartedFlag;

    private void ResetAllFlags()
    {
        finishedInteractionFlag = false;
        PlayerInteractFlagSet = false;
        ButtonClickedFlagSet = false;
        newDialogueStartedFlag = false;
    }

    public void OnDialogueButtonSelected()
    {
        ButtonClickedFlagSet = true;
        lastHighlightedItem = this.UIEventSystem.currentSelectedGameObject;
    }

    private void PlayDialoguePrintAudio()
    {
        if (this.audioSource.clip != this.dialoguePrintingAudio)
        {
            this.audioSource.clip = this.dialoguePrintingAudio;
        }

        this.audioSource.Play();
    }

    public void BeginDialogue(DialogueSlide firstSlide, Unit_NPC speaker)
    {
        this.newDialogueStartedFlag = true;
        currentSpeaker = speaker;
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
        this.audioSource.loop = false;
    }

    private void UpdateCurrentSlide(DialogueSlide? newSlide)
    {
        if (newSlide == null)
        {
            this.CurrentSlide = null;
        }
        else
        {
            this.CurrentSlide = newSlide;
        }

        speakerNameText.text = currentSpeaker.unitName;
    }

    private void Update()
    {
        switch (State)
        {
            case BoxState.WAITINGFORINTERACTION:
                if (this.newDialogueStartedFlag)
                {
                    DeactivateAllButtons();
                    this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                    this.FinishedWritingSlideOverTime = false;
                    this.State = BoxState.WRITINGSLIDE;
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
                    MyGuard.IsNotNull(this.CurrentSlide);
                    // if no options on slide and player clicked then they want to go to the next slide or end dialogue.
                    if (!this.CurrentSlide.dialogueOptions.Any())
                    {
                        FinishedWithSlide(this.CurrentSlide.nextSlide);
                        return;
                    }
                }
                else if (this.ButtonClickedFlagSet)
                {
                    MyGuard.IsNotNull(this.CurrentSlide);
                    var buttonOption = lastHighlightedItem.GetComponentInParent<DialogueOptionButton>();
                    MyGuard.IsNotNull(buttonOption, "Button clicked flag set cannot have no button clicked");
                    FinishedWithSlide(buttonOption.NextDialogueSlide);
                }
                break;
            default:
                State = BoxState.WAITINGFORINTERACTION;
                break;
        }

        ResetAllFlags();
    }

    private void StartNextSlide(DialogueSlide nextSlide)
    {
        // var selectedDiaOption = this.buttons.First(x => x.GetComponent<DialogueOptionButton>().isSelected);
        this.UpdateCurrentSlide(nextSlide);
        this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
        //Log.Print("state writing after waiting");
        this.State = BoxState.WRITINGSLIDE;
    }

    private void EndConversation()
    {
        UpdateCurrentSlide(null);
        //Log.Print("state INVIS INACTIVE");
        finishedInteractionFlag = true;
        this.State = BoxState.WAITINGFORINTERACTION;
        this.gameObject.SetActive(false);
    }

    private void StartFight()
    {
        var player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull(player);
        SaveGameUtility.SaveGame(player);

        if (player.InteractableInRange is Unit_NPC enemy)
        {
            MyGuard.IsNotNull(enemy);
            MyGuard.IsNotNull(player.enemyLoader);
            player.enemyLoader.enemyWasTalkingTo = enemy.gameObject;
            DontDestroyOnLoad(enemy.gameObject);
        }

        TalkingDungeonScenes.LoadScene(TalkingDungeonScenes.Battle, this.transitionForGoingToBattleScene, SaveGameState.StartedBattle);
        return;
    }

    /// <summary>
    /// When player Clicked on the slide with no options potentialnext slide = currentslide.next.
    /// Or when clicked on an option the potential slide = button option slide.
    /// </summary>
    /// <param name="potentialNextSlide"></param>
    private void FinishedWithSlide(DialogueSlide? potentialNextSlide)
    {
        MyGuard.IsNotNull(CurrentSlide);
        Debug.Log("If i want only a specific dialogue option to start a fight add that here");
        if (this.CurrentSlide.startFight)
        {
            StartFight();
        }
        else if (potentialNextSlide != null)
        {
            StartNextSlide(potentialNextSlide);
        }
        else
        {
            EndConversation();
        }
    }

    private void SkipToEnd()
    {
        // In more recent versions of Unity (at least 5.3 onwards) you can keep a reference to the IEnumerator or returned Coroutine object and start and stop that directly, rather than use the method name. These are preferred over using the method name as they are type safe and more performant. See the StopCoroutine docs for details https://docs.unity3d.com/ScriptReference/MonoBehaviour.StopCoroutine.html
        StopCoroutine(writeSlidesOverTimeCoroutine);
        DrawButtons();
        string parsedString = "";
        MyGuard.IsNotNull(CurrentSlide);
        MyGuard.IsNotNull(CurrentSlide.dialogue);
        foreach (var item in CurrentSlide.dialogue)
        {
            if (item != pauseCharacterToNotPrint)
            {
                parsedString += item;
            }
        }

        this.TMPTextBox.text = parsedString;
    }

    /// <summary>
    /// Draws buttons i.e. instantiates them in the button locations. TODO: change this to not instantiate but to populate values of already loaded button objects.
    /// </summary>
    private void DrawButtons()
    {
        MyGuard.IsNotNull(this.CurrentSlide);
        if (this.CurrentSlide.dialogueOptions == null || this.CurrentSlide.dialogueOptions.Count == 0)
        {
            return;
        }

        for (var i = 0; i < this.CurrentSlide.dialogueOptions.Count; i++)
        {
            this.buttons[i].gameObject.SetActive(true);
            this.buttons[i].SetValues(this.CurrentSlide.dialogueOptions[i].optionText, this.CurrentSlide.dialogueOptions[i].nextSlide);
        }

        // need to do this to stop the bug where if a mouse is above the spot a button will appear the button gets stuck on highlighted.
        this.UIEventSystem.SetSelectedGameObject(null);
    }

    private void DeactivateAllButtons()
    {
        foreach (var button in this.buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private IEnumerator WriteSlideOverTime()
    {
        DeactivateAllButtons();
        MyGuard.IsNotNull(this.CurrentSlide);
        MyGuard.IsNotNull(this.CurrentSlide.dialogue);

        // get the autosized font size. then reprint the text at that size without autosize enabled so it doesnt change size while printing.
        this.TMPTextBox.enableAutoSizing = true;
        this.TMPTextBox.text = this.CurrentSlide.dialogue;
        this.TMPTextBox.ForceMeshUpdate();
        var fontSize = this.TMPTextBox.fontSize;
        this.TMPTextBox.text = string.Empty;
        this.TMPTextBox.enableAutoSizing = false;
        this.TMPTextBox.fontSize = fontSize;
        this.TMPTextBox.ForceMeshUpdate();

        for (int i = 0; i < this.CurrentSlide.dialogue.Length; i++)
        {
            // don't play sound for either the special pause text printing character, or spaces. 
            if (this.CurrentSlide.dialogue[i] == pauseCharacterToNotPrint)
            {
                yield return new WaitForSeconds(underscorePauseTime);
                this.audioSource.Pause();
                continue;
            }

            // dont play a sound but do print a space empty char
            if (this.CurrentSlide.dialogue[i] == ' ')
            {
                this.TMPTextBox.text += this.CurrentSlide.dialogue[i];
                this.audioSource.Pause();
                yield return new WaitForSeconds(textspeed);
                continue;
            }

            this.PlayDialoguePrintAudio();
            if (i == 0) // set first letter if this is the first letter.
            {
                this.TMPTextBox.SetText(this.CurrentSlide.dialogue[0].ToString());
                continue;
            }

            // do the rest of the letters
            this.TMPTextBox.text += this.CurrentSlide.dialogue[i];
            yield return new WaitForSeconds(textspeed);
        }

        this.audioSource.Stop();
        DrawButtons();
        this.FinishedWritingSlideOverTime = true;
    }
}
