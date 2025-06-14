using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
#nullable enable

/// <summary>
/// Represents a text box class that does the dialogue options and prints text.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DialogueTextBox : Textbox
{
    public TransitionSettings transitionForGoingToBattleScene;
    [SerializeField] private List<DialogueOptionButton> buttons;
    public TMP_Text speakerNameText;
    protected DialogueSlide? CurrentSlide { get; set; }
    protected Unit_NPC? currentSpeaker;

    protected override void ResetAllFlags()
    {
        ButtonClickedFlagSet = false;
    }

    private void Update()
    {
        switch (State)
        {
            case BoxState.INACTIVE:
                break;
            case BoxState.WRITINGSLIDE:
                if (this.InteractFlag)
                {
                    this.SkipToEnd();
                }
                if (this.finishedWritingSlide)
                {
                    DrawButtons();
                    this.State = BoxState.WAITINGONSLIDE;
                }
                break;
            case BoxState.WAITINGONSLIDE:
                if (this.InteractFlag && this.CurrentSlide != null && !this.CurrentSlide.dialogueOptions.Any())
                {
                    MyGuard.IsNotNull(this.CurrentSlide);
                    // if no options on slide and player clicked then they want to go to the next slide or end dialogue.
                    FinishedWaitingOnSlide(this.CurrentSlide.nextSlide);
                    return;
                }
                else if (this.ButtonClickedFlagSet)
                {
                    MyGuard.IsNotNull(this.CurrentSlide);
                    var buttonOption = lastHighlightedItem.GetComponentInParent<DialogueOptionButton>();
                    MyGuard.IsNotNull(buttonOption, "Button clicked flag set cannot have no button clicked");
                    FinishedWaitingOnSlide(buttonOption.NextDialogueSlide);
                }
                break;
            default:
                State = BoxState.INACTIVE;
                break;
        }

        ResetAllFlags();
    }


    public void OnDialogueButtonSelected()
    {
        ButtonClickedFlagSet = true;
        lastHighlightedItem = this.UIEventSystem.currentSelectedGameObject;
    }

    public void BeginDialogue(DialogueSlide firstSlide, Unit_NPC speaker)
    {
        this.currentSpeaker = speaker;
        this.UpdateCurrentSlide(firstSlide);
        this.State = BoxState.WRITINGSLIDE;
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
        MyGuard.IsNotNull(currentSpeaker);
        speakerNameText.text = currentSpeaker.unitName;
    }


    private void GotoNextSlide(DialogueSlide nextSlide)
    {
        DeactivateAllButtons();
        this.UpdateCurrentSlide(nextSlide);
        this.StartWriting(nextSlide.dialogue, Color.black);
    }

    private void EndConversation()
    {
        UpdateCurrentSlide(null);
        this.State = BoxState.INACTIVE;
        this.gameObject.SetActive(false);
        MyGuard.IsNotNull(currentSpeaker);
        currentSpeaker.EndInteract();
        this.currentSpeaker = null;
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
    private void FinishedWaitingOnSlide(DialogueSlide? potentialNextSlide)
    {
        MyGuard.IsNotNull(CurrentSlide);
        Debug.Log("If i want only a specific dialogue option to start a fight add that here");
        if (this.CurrentSlide.startFight)
        {
            StartFight();
        }
        else if (potentialNextSlide != null)
        {
            GotoNextSlide(potentialNextSlide);
        }
        else
        {
            EndConversation();
        }
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
}
