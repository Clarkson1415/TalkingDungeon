using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SaveGameUtility;
#nullable enable

public class BattleUI : MenuWithButtons
{
    [Header("Player")]
    private PlayerDungeon player;
    [SerializeField] private GameObject PlayerHealthBarAndNameToShake;
    [SerializeField] private Image PlayerHealthFill;
    [HideInInspector] public bool IsPlayerHealthBarPlayingAnimation => player.healthBarFill.fillAmount > Mathf.Clamp((player.currentHealth / player.maxHealth), 0, 1);
    [HideInInspector] public bool IsEnemyHealthAnimationPlaying => enemyYouFightin.healthBarFill.fillAmount > Mathf.Clamp((enemyYouFightin.currentHealth / enemyYouFightin.maxHealth), 0, 1);

    [Header("UI")]
    [SerializeField] TransitionSettings exitBattleTransition;
    [SerializeField] AudioSource buttonClickedAudioSource;

    [SerializeField] private List<InventorySlot> AbilityButtons;
    [SerializeField] GameObject actionButtonScreen;
    [SerializeField] GameObject abilityButtonSceen;
    [SerializeField] GameObject itemScreen;
    [SerializeField] GameObject runScreen;
    [SerializeField] GameObject talkScreen;
    [SerializeField] private GameObject battleDialogueBox;
    private TMP_Text battleDialogBoxAboveText;
    [SerializeField] GameObject backButton;

    [Header("Enemy")]
    private Unit_NPC enemyYouFightin;
    [SerializeField] private TMP_Text enemyNameField;
    [SerializeField] private GameObject enemyHealthBarAndNameToShake;
    [SerializeField] private Image enemyHealthFill;

    private Battle state;
    private EventSystem evSys;

    private static Color positiveGreen = new Color(0, 0.7f, 0);

    [Header("Dialogue Printing Stuff")]
    private const char pauseCharacterToNotPrint = '_';
    [SerializeField] private float underscorePauseTime = 0.05f;
    private AudioSource dialoguePrintAudio;
    [SerializeField] private TMP_Text TopSlideText;
    [SerializeField] private float timeBetweenLetters;

    private bool actionClickedFlag;
    private bool abilityClickedFlag;
    private bool backButtonClickedFlag;
    private bool isDialoguePrinting;

    private void ResetFlags()
    {
        actionClickedFlag = false;
        abilityClickedFlag = false;
        backButtonClickedFlag = false;
    }

    private void Awake()
    {
        battleDialogueBox.SetActive(true);
        battleDialogBoxAboveText = this.battleDialogueBox.GetComponentInChildren<TMP_Text>();
        dialoguePrintAudio = this.gameObject.GetComponent<AudioSource>();
    }
    
    
    private IEnumerator TestDialogueBox(string text, Color color)
    {
        // get the autosized font size. then reprint the text at that size without autosize enabled so it doesnt change size while printing.
        this.TopSlideText.enableAutoSizing = true;
        this.TopSlideText.text = text;
        this.TopSlideText.ForceMeshUpdate();
        var fontSize = this.TopSlideText.fontSize;
        this.TopSlideText.text = string.Empty;
        this.TopSlideText.enableAutoSizing = false;
        this.TopSlideText.fontSize = fontSize;
        this.TopSlideText.ForceMeshUpdate();

        var secondsToWait = text.Length / 25f;
        this.isDialoguePrinting = true;
        battleDialogBoxAboveText.text = text;
        battleDialogBoxAboveText.color = color;

        for (int i = 0; i < text.Length; i++)
        {
            // don't play sound for either the special pause text printing character, or spaces. 
            if (text[i] == pauseCharacterToNotPrint)
            {
                yield return new WaitForSeconds(underscorePauseTime);
                this.dialoguePrintAudio.Pause();
                continue;
            }

            // dont play a sound but do print a space empty char
            if (text[i] == ' ')
            {
                this.TopSlideText.text += text[i];
                this.dialoguePrintAudio.Pause();
                yield return new WaitForSeconds(timeBetweenLetters);
                continue;
            }

            this.dialoguePrintAudio.Play();
            if (i == 0) // set first letter if this is the first letter.
            {
                this.TopSlideText.SetText(text[0].ToString());
                continue;
            }

            // do the rest of the letters
            this.TopSlideText.text += text[i];
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        yield return new WaitForSeconds(secondsToWait);
        this.isDialoguePrinting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        evSys = FindObjectOfType<EventSystem>();
        player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull(player, "could not find player");
        player.healthBarFill = this.PlayerHealthFill;
        player.HealthBarObject = this.PlayerHealthBarAndNameToShake;
        state = Battle.PlayerPickActionTurn;
        // TODO do i want the dialogue box to maybe say stuff on opening, like enemy approached...
        this.abilityButtonSceen.SetActive(false);
        this.actionButtonScreen.SetActive(true);
        this.backButton.SetActive(false);
        itemScreen.SetActive(false);
        talkScreen.SetActive(false);
        runScreen.SetActive(false);
        StartCoroutine(TestDialogueBox("Your turn", Color.black));
    }

    public void SetupEnemyAfterSpawned()
    {
        enemyYouFightin = FindObjectOfType<Unit_NPC>();
        if (enemyYouFightin == null)
        {
            throw new ArgumentNullException("enemy cannot be null in battle scene.");
        }

        enemyNameField.text = this.enemyYouFightin.unitName;
        enemyYouFightin.HealthBarObject = this.enemyHealthBarAndNameToShake;
        enemyYouFightin.healthBarFill = this.enemyHealthFill;
        this.enemyYouFightin.SetupUnitForBattle();
        this.actionButtonScreen.SetActive(true);
    }

    private enum Battle
    {
        PlayerPickActionTurn,
        PlayerPickAbilityTurn,
        ExecutingPlayersMove,
        EnemyTurn,
        ExecutingEnemiesMove,
        PlayerWon,
        PlayerLost,
        WaitOnDeathScreen,
        RunAwaySuccess,
        TransitioningOutOfBattle,
        Paused,
        inItemMenu,
        InTalkMenu,
    }

    private void SetupAbilityButtons()
    {
        // TODO setup ability buttons
        // idk if this will change mid battle otherwise can do on start in an ability UI class on the ability UI object. or in here on start would be better maybe?
        if (player.Abilities.Count > 4)
        {
            throw new ArgumentException("abilities is more than supported in battle UI which is 4");
        }

        for (int i = 0; i < player.Abilities.Count; i++)
        {
            this.AbilityButtons[i].gameObject.SetActive(true);
            this.AbilityButtons[i].SetAbilityAndImage(player.Abilities[i], player.equippedWeapon);
        }

        for (int j = player.Abilities.Count; j < this.AbilityButtons.Count; j++)
        {
            this.AbilityButtons[j].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame 
    void Update()
    {
        if (this.isDialoguePrinting)
        {
            return;
        }

        switch (state)
        {
            case Battle.PlayerPickActionTurn:
                if (this.actionClickedFlag)
                {
                    this.actionClickedFlag = false;

                    TurnBasedActions? action = null;
                    if (this.evSys.currentSelectedGameObject.TryGetComponent<TurnBasedActionButton>(out var turnBasedButton))
                    {
                        action = turnBasedButton.Action;
                    }

                    if (action == null)
                    {
                        throw new ArgumentNullException("Action was null in battle UI this is not meant to happen.");
                    }

                    this.actionButtonScreen.SetActive(false);
                    backButton.SetActive(true);

                    switch (action)
                    {
                        case TurnBasedActions.Attack:
                            this.abilityButtonSceen.SetActive(true);
                            SetupAbilityButtons();
                            this.state = Battle.PlayerPickAbilityTurn;
                            break;
                        case TurnBasedActions.Run:
                            backButton.SetActive(false);
                            bool runSuccesss = true;
                            this.runScreen.SetActive(true);
                            if (player.currentHealth <= (enemyYouFightin.currentHealth))
                            {
                                Debug.Log("Getaway based on if you have more wellbeing than enemy");
                                runSuccesss = false;
                            }

                            if (runSuccesss)
                            {
                                StartCoroutine(TestDialogueBox("you got away!", Color.black));
                                this.state = Battle.RunAwaySuccess;
                            }
                            else
                            {
                                StartCoroutine(TestDialogueBox("Failed to get away.", Color.red));
                                this.state = Battle.EnemyTurn;
                            }
                            break;
                        case TurnBasedActions.Item:
                            itemScreen.SetActive(true);
                            backButton.SetActive(true);
                            state = Battle.inItemMenu;
                            Debug.Log("TODO will be able to use Item equipped or use a turn to equip an item.");
                            // either use your equipped item or use a turn to change equipped item
                            break;
                        case TurnBasedActions.Talk:
                            // TODO 
                            talkScreen.SetActive(true);
                            backButton.SetActive(true);
                            state = Battle.InTalkMenu;
                            // StartDialogue(this.enemyYouFightin.battleSceneDialogueSlide);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("No matching action.");
                    }
                }
                break;
            case Battle.inItemMenu:
                if (this.backButtonClickedFlag)
                {
                    BackCLickedGoBackToPlayerAction(this.itemScreen);
                }
                break;
            case Battle.InTalkMenu:
                if (this.backButtonClickedFlag)
                {
                    BackCLickedGoBackToPlayerAction(this.talkScreen);
                }
                break;
            case Battle.PlayerPickAbilityTurn:
                if (this.abilityClickedFlag)
                {
                    var abilityUsed = evSys.currentSelectedGameObject.GetComponent<InventorySlot>().Ability;
                    MyGuard.IsNotNull(abilityUsed, "AbilityUsed battle Ui in playerPickAbilityTurn was null.");
                    Log.Print($"You used {abilityUsed.name} on {enemyYouFightin.unitName} To {abilityUsed.Effects}");
                    abilityUsed.Apply(player.equippedWeapon, player, enemyYouFightin);
                    ShowAbilityUsedText(this.player, abilityUsed);
                    this.abilityButtonSceen.SetActive(false);
                    this.backButton.SetActive(false);
                    this.state = Battle.ExecutingPlayersMove;
                }
                if (this.backButtonClickedFlag)
                {
                    BackCLickedGoBackToPlayerAction(this.abilityButtonSceen);
                }
                break;
            case Battle.ExecutingPlayersMove:
                // wait until enemy health bar anim finished then take enemies turn
                // when finished showing player move text go to enemy move
                // also need to wait for player health bar anim for healing etc.
                // also somehow will need to Wait for whatever effect animation is playing.
                // maybe use animation state but need to make 1 anim override for effect animations then.
                if (!IsEnemyHealthAnimationPlaying && !IsPlayerHealthBarPlayingAnimation && !isDialoguePrinting)
                {
                    if (enemyYouFightin.currentHealth > 0)
                    {
                        StartCoroutine(TestDialogueBox("Enemy Turn", Color.black));
                        this.state = Battle.EnemyTurn;
                    }
                    else
                    {
                        Debug.Log("player won");
                        StartCoroutine(TestDialogueBox("You Won", Color.black));
                        var scene = enemyYouFightin.SceneAfterWin;
                        TalkingDungeonScenes.LoadScene(scene, exitBattleTransition, GameState.BattleWon);
                        this.state = Battle.PlayerWon;
                    }
                }
                break;
            case Battle.EnemyTurn:
                // or enemy could use an item.
                Debug.Log("not finished setup here. need to calculate damage based on units current defence stat also?");
                var enemyAbility = PickRandomAbility(this.enemyYouFightin.Abilities);
                enemyAbility.Apply(enemyYouFightin.equippedWeapon, enemyYouFightin, player);
                ShowAbilityUsedText(this.enemyYouFightin, enemyAbility);
                // TODO display damage turn text on screen
                Log.Print("current wellbeing " + this.player.currentHealth);

                if (this.player.currentHealth <= 0)
                {
                    // TODO note this will not take into account the animation perhaps I could speed it up if the player health will be dead
                    state = Battle.PlayerLost;
                }
                else
                {
                    state = Battle.ExecutingEnemiesMove;
                }
                break;
            case Battle.ExecutingEnemiesMove:
                // when dialogue not printing and player animation finished  
                if (!this.IsPlayerHealthBarPlayingAnimation && !this.IsEnemyHealthAnimationPlaying && !isDialoguePrinting)
                {
                    StartCoroutine(TestDialogueBox("Your Turn", Color.black));
                    BackCLickedGoBackToPlayerAction(this.runScreen);
                }
                break;
            case Battle.PlayerWon:
                break;
            case Battle.PlayerLost:
                if (!this.IsPlayerHealthBarPlayingAnimation && !isDialoguePrinting) // when dialogue finished printing display death
                {
                    Debug.Log("in playerlost goto wait on death screen");
                    state = Battle.WaitOnDeathScreen;
                }
                break;
            case Battle.RunAwaySuccess:
                if (!this.isDialoguePrinting)
                {
                    // TODO play sound effect for running away
                    // also only need to save current player health and inventory items they might have used items.
                    SaveGameUtility.SaveStuffFromBattle(player);
                    var scenePlayerSavedInLast = PlayerPrefs.GetString(SaveKeys.LastScene);
                    TalkingDungeonScenes.LoadScene(scenePlayerSavedInLast, exitBattleTransition, GameState.BattleRunAwaySuccess);
                    this.state = Battle.TransitioningOutOfBattle;
                }
                break;
            case Battle.WaitOnDeathScreen:
                break;
            case Battle.TransitioningOutOfBattle:
                break;
            default:
                break;
        }

        ResetFlags();
    }

    private void BackCLickedGoBackToPlayerAction(GameObject screenToDeactivate)
    {
        screenToDeactivate.SetActive(false);
        this.actionButtonScreen.SetActive(true);
        state = Battle.PlayerPickActionTurn;
        backButton.SetActive(false);
    }

    private void ShowAbilityUsedText(Unit user, Ability abilityUsed)
    {
        var color = positiveGreen;
        var person = "Player";
        if (user is Unit_NPC)
        {
            color = Color.red;
            person = enemyYouFightin.unitName;
        }

        var turnInfoString = $"{person} used {user.equippedWeapon.Name} to {abilityUsed.Name} to {abilityUsed.FormatDescription(user.equippedWeapon)}";
        StartCoroutine(TestDialogueBox(turnInfoString, color));
    }

    private static readonly System.Random Random = new();

    private Ability PickRandomAbility(List<Ability> abilities)
    {
        var abilityIndex = Random.Next(0, this.enemyYouFightin.Abilities.Count);
        return abilities[abilityIndex];
    }

    private void StartDialogue(DialogueSlide dialogue)
    {
        throw new NotImplementedException();
    }

    public void OnActionButtonClicked()
    {
        this.actionClickedFlag = true;
        buttonClickedAudioSource.Play();
    }

    public void OnAbilityButtonClicked()
    {
        this.abilityClickedFlag = true;
        buttonClickedAudioSource.Play();
    }

    public void OnBackButtonClicked()
    {
        backButtonClickedFlag = true;
        buttonClickedAudioSource.Play();
    }
}
