using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#nullable enable

[RequireComponent(typeof(AudioSource))]
public class DialogueTextBox : MonoBehaviour
{
    [Header("Battle Stuff")]
    public TransitionSettings transitionForGoingToBattleScene;

    public DialogueSlide? CurrentSlide { get; private set; }
    public BoxState State { get; private set; } = BoxState.WAITINGFORINTERACTION;
    [SerializeField] private float textspeed = 0.1f;
    [SerializeField] private float underscorePauseTime = 0.01f;
    [SerializeField] GameObject prefabButton;
    [SerializeField] private EventSystem UIEventSystem;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<GameObject> buttonPositionsTopToBottom;
    [SerializeField] AudioClip dialoguePrintingAudio;
    
    readonly List<GameObject> buttons = new();
    [HideInInspector] public bool PlayerInteractFlagSet;
    private Coroutine? writeSlidesOverTimeCoroutine = null;
    private bool newInteractionSetup;
    private bool FinishedWritingSlideOverTime;
    private const char pauseCharacterToNotPrint = '_';
    private TMP_Text TMPTextBox;

    /// <summary>
    /// for the player statemachine to recognise the interaction has finished.
    /// </summary>
    [HideInInspector] public bool finishedInteractionFlag;

    private void PlayDialoguePrintAudio()
    {
        if (this.audioSource.clip != this.dialoguePrintingAudio)
        {
            this.audioSource.clip = this.dialoguePrintingAudio;
        }

        this.audioSource.Play();
    }

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
    }

    private GameObject? currentHighlightedbutton;

    private void Update()
    {
        // currentHighlightedbutton starts off null and i dont think i want the change selection sound to play when the buttons are instantiated.
        var highlightedMenuItem = this.UIEventSystem.currentSelectedGameObject;

        if (highlightedMenuItem != currentHighlightedbutton && currentHighlightedbutton != null)
        {
            if(highlightedMenuItem != null)
            {
                this.currentHighlightedbutton = highlightedMenuItem;
                if (this.currentHighlightedbutton.TryGetComponent<DialogueOptionButton>(out var button))
                {
                    button.PlayHighlightOptionChangedSound();
                }
            }
        }

        switch (State)
        {
            case BoxState.WAITINGFORINTERACTION:
                if (this.PlayerInteractFlagSet && this.newInteractionSetup)
                {
                    State = BoxState.WRITINGSLIDE;
                    this.newInteractionSetup = false;
                    this.PlayerInteractFlagSet = false;
                    this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                    this.FinishedWritingSlideOverTime = false;
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
                    MyGuard.IsNotNull(this.CurrentSlide);
                    
                    if (this.CurrentSlide.startFight)
                    {
                        var player = FindObjectOfType<PlayerDungeon>();
                        MyGuard.IsNotNull(player);
                        PlayerDataUtility.SaveGame(player);

                        if (player.InteractableInRange is Unit enemy)
                        {
                            MyGuard.IsNotNull(enemy);
                            MyGuard.IsNotNull(player.enemyLoader);
                            MyGuard.IsNotNull(enemy.prefabToUseInBattle);
                            player.enemyLoader.enemyWasTalkingTo = enemy.gameObject;
                            DontDestroyOnLoad(enemy.gameObject);
                        }

                        TransitionManager.Instance().Transition(TalkingDungeonScenes.Battle, this.transitionForGoingToBattleScene, 0f);
                        return;
                    }

                    // if its null no buttons so either go to next slide or exit dialogue

                    if (this.UIEventSystem.currentSelectedGameObject == null && this.CurrentSlide.nextSlide != null)
                    {
                        // var selectedDiaOption = this.buttons.First(x => x.GetComponent<DialogueOptionButton>().isSelected);
                        this.UpdateCurrentSlide(CurrentSlide.nextSlide);
                        this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                        //Log.Print("state writing after waiting");
                        this.State = BoxState.WRITINGSLIDE;
                    }
                    else if (this.UIEventSystem.currentSelectedGameObject == null)
                    {
                        UpdateCurrentSlide(null);
                        //Log.Print("state INVIS INACTIVE");
                        finishedInteractionFlag = true;
                        this.State = BoxState.WAITINGFORINTERACTION;
                        Debug.Log("text box state set to waiting for interaction");
                        this.gameObject.SetActive(false);
                    }
                    else // a button was clicked
                    {
                        var selected = this.UIEventSystem.currentSelectedGameObject;
                        if (!selected.TryGetComponent<DialogueOptionButton>(out var buttonOption))
                        {
                            Debug.Log("could not deg a dialouge option button component");
                        }

                        if (buttonOption.NextDialogueSlide == null)
                        {
                            // end converstiona
                            // TODO THIS AS A FUCNTION USED ABOVE
                            UpdateCurrentSlide(null);
                            //Log.Print("state INVIS INACTIVE");
                            finishedInteractionFlag = true;
                            this.State = BoxState.WAITINGFORINTERACTION;
                            Debug.Log("Box in Set to waiting for interaction");
                            this.gameObject.SetActive(false);
                        }
                        else
                        {
                            // var selectedDiaOption = this.buttons.First(x => x.GetComponent<DialogueOptionButton>().isSelected);
                            this.UpdateCurrentSlide(buttonOption.NextDialogueSlide);
                            this.writeSlidesOverTimeCoroutine = StartCoroutine(WriteSlideOverTime());
                            //Log.Print("state writing after waiting");
                            this.State = BoxState.WRITINGSLIDE;
                        }
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

        if (this.CurrentSlide.dialogueOptions.Count > 0)
        {
            for (int i = 0; i < this.CurrentSlide.dialogueOptions.Count; i++)
            {
                // Instantiate new button with that gameobject as parent.
                var buttonGameObj = Instantiate(this.prefabButton, this.buttonPositionsTopToBottom[i].transform);

                // set button Dialogue Option to the Dialogue Option.
                // REMEBER THIS IS NOT THE SAME OBJECT AS IN THE CURRENT SLIDE.OPTIONS
                buttonGameObj.GetComponent<DialogueOptionButton>().SetValues(this.CurrentSlide.dialogueOptions[i].optionText, this.CurrentSlide.dialogueOptions[i].nextSlide);
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

        this.currentHighlightedbutton = this.UIEventSystem.currentSelectedGameObject;
    }

    private IEnumerator WriteSlideOverTime()
    {
        // remove old buttons 
        this.buttons.ForEach(x => Destroy(x));
        this.buttons.Clear();

        MyGuard.IsNotNull(this.CurrentSlide);
        MyGuard.IsNotNull(this.CurrentSlide.dialogue);
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

            // play dialogue text sound effect

            // this was in here for the writing loop but idk if ill use it
            //if (!this.audioSource.isPlaying)
            //{
            //    this.PlayDialoguePrintAudio();
            //}
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
