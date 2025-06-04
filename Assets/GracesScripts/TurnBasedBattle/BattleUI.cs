using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using EasyTransition;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveGameUtility;
#nullable enable

public class BattleUI : MenuWithButtons
{
    [Header("Player")]
    private PlayerDungeon player;
    [SerializeField] private GameObject PlayerHealthBarAndNameToShake;
    [SerializeField] private Image PlayerHealthFill;
    private bool IsPlayerHealthBarPlayingAnimation => player.healthBarFill.fillAmount > Mathf.Clamp((player.currentHealth / player.maxHealth), 0, 1);
    private bool IsEnemyHealthAnimationPlaying => enemyYouFightin.healthBarFill.fillAmount > Mathf.Clamp((enemyYouFightin.currentHealth / enemyYouFightin.maxHealth), 0, 1);
    private bool AreAnimationsFinished =>
    !IsPlayerHealthBarPlayingAnimation &&
    !IsEnemyHealthAnimationPlaying &&
    !isDialoguePrinting;

    [Header("UI")]
    [SerializeField] TransitionSettings exitBattleTransition;
    [SerializeField] AudioSource buttonClickedAudioSource;

    [SerializeField] private List<InventorySlot> AbilityButtons;
    [SerializeField] GameObject actionButtonScreen;
    [SerializeField] GameObject abilityButtonScreen;
    [SerializeField] GameObject itemScreen;
    [SerializeField] GameObject runScreen;
    [SerializeField] GameObject talkScreen;
    [SerializeField] private GameObject battleDialogueBox;
    private TMP_Text battleDialogBoxAboveText;
    [SerializeField] GameObject backButton;

    private List<GameObject> ScreensNotAction => new() { this.itemScreen, this.runScreen, this.abilityButtonScreen, this.talkScreen };


    [Header("Enemy")]
    private Unit_NPC enemyYouFightin;
    [SerializeField] private TMP_Text enemyNameField;
    [SerializeField] private GameObject enemyHealthBarAndNameToShake;
    [SerializeField] private Image enemyHealthFill;

    private Battle state;
    private static Color positiveGreen = new Color(0, 0.7f, 0);

    [Header("Dialogue Printing Stuff")]
    private const char pauseCharacterToNotPrint = '_';
    [SerializeField] private float underscorePauseTime = 0.05f;
    private AudioSource dialoguePrintAudio;
    [SerializeField] private TMP_Text TopSlideText;
    [SerializeField] private float timeBetweenLetters;

    private bool isDialoguePrinting;

    private void Awake()
    {
        battleDialogueBox.SetActive(true);
        battleDialogBoxAboveText = this.battleDialogueBox.GetComponentInChildren<TMP_Text>();
        dialoguePrintAudio = this.gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull(player, "could not find player");
        player.healthBarFill = this.PlayerHealthFill;
        player.HealthBarObject = this.PlayerHealthBarAndNameToShake;
        state = Battle.PlayerTurn;
        // TODO do i want the dialogue box to maybe say stuff on opening, like enemy approached...
        this.abilityButtonScreen.SetActive(false);
        this.actionButtonScreen.SetActive(true);
        this.backButton.SetActive(false);
        itemScreen.SetActive(false);
        talkScreen.SetActive(false);
        runScreen.SetActive(false);
        StartCoroutine(TestDialogueBox("Your turn", Color.black));
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
        PlayerTurn,
        FinishedPLayerTurn,
        EnemyPickAbilityTurn,
        FinishedEnemiesTurn,
        WaitOnDeathScreenOrTransitioning,
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
        if (!this.AreAnimationsFinished)
        {
            return;
        }

        HandleBattleState();
    }

    private void HandleActionButtonClicked()
    {
        TurnBasedActions? action = null;
        if (this.UIEventSystem.currentSelectedGameObject.TryGetComponent<TurnBasedActionButton>(out var turnBasedButton))
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
                this.abilityButtonScreen.SetActive(true);
                SetupAbilityButtons();
                break;
            case TurnBasedActions.Run:
                backButton.SetActive(false);
                this.runScreen.SetActive(true);
                bool runSuccesss = true;
                if (player.currentHealth <= (enemyYouFightin.currentHealth))
                {
                    Debug.Log("Getaway based on if you have more wellbeing than enemy");
                    runSuccesss = false;
                }

                if (runSuccesss)
                {
                    StartCoroutine(TestDialogueBox("you got away!", Color.black));
                    // TODO play sound effect for running away
                    SaveGameUtility.SaveStuffFromBattle(player);
                    var scenePlayerSavedInLast = PlayerPrefs.GetString(SaveKeys.LastScene);
                    TalkingDungeonScenes.LoadScene(scenePlayerSavedInLast, exitBattleTransition, SaveGameState.BattleRunAwaySuccess);
                    this.state = Battle.WaitOnDeathScreenOrTransitioning;
                }
                else
                {
                    StartCoroutine(TestDialogueBox("Failed to get away.", Color.red));
                    this.state = Battle.EnemyPickAbilityTurn;
                }
                break;
            case TurnBasedActions.Item:
                itemScreen.SetActive(true);
                backButton.SetActive(true);
                Debug.Log("TODO will be able to use Item equipped or use a turn to equip an item.");
                // either use your equipped item or use a turn to change equipped item
                break;
            case TurnBasedActions.Talk:
                // TODO 
                talkScreen.SetActive(true);
                backButton.SetActive(true);
                // StartDialogue(this.enemyYouFightin.battleSceneDialogueSlide);
                break;
            default:
                throw new ArgumentOutOfRangeException("No matching action.");
        }
    }

    private void HandleBattleState()
    {
        switch (state)
        {
            case Battle.PlayerTurn:
                break;
            case Battle.FinishedPLayerTurn:
                if (enemyYouFightin.currentHealth > 0)
                {
                    StartCoroutine(TestDialogueBox("Enemy Turn", Color.black));
                    this.state = Battle.EnemyPickAbilityTurn;
                }
                else
                {
                    StartCoroutine(TestDialogueBox("You Won", Color.black));
                    var scene = enemyYouFightin.SceneAfterWin;
                    TalkingDungeonScenes.LoadScene(scene, exitBattleTransition, SaveGameState.BattleWon);
                    this.state = Battle.WaitOnDeathScreenOrTransitioning;
                }
                break;
            case Battle.EnemyPickAbilityTurn:
                // or enemy could use an item.
                Debug.Log("not finished setup here. need to calculate damage based on units current defence stat also?");
                var enemyAbility = PickRandomAbility(this.enemyYouFightin.Abilities);
                enemyAbility.Apply(enemyYouFightin, player);
                ShowAbilityUsedText(this.enemyYouFightin, enemyAbility);
                // TODO display damage turn text on screen
                state = this.player.currentHealth <= 0 ? Battle.WaitOnDeathScreenOrTransitioning : Battle.FinishedEnemiesTurn;
                break;
            case Battle.FinishedEnemiesTurn:
                StartCoroutine(TestDialogueBox("Your Turn", Color.black));
                OnBackButtonClicked();
                break;
            case Battle.WaitOnDeathScreenOrTransitioning:
                break;
            default:
                break;
        }
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

        var turnInfoString = $"{person} used {user.equippedWeapon.Name} to {abilityUsed.Name} to {abilityUsed.FormatDescription(user.equippedWeapon, user)}";
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
        buttonClickedAudioSource.Play();
        HandleActionButtonClicked();
    }

    public void OnAbilityButtonClicked()
    {
        buttonClickedAudioSource.Play();

        var abilityUsed = UIEventSystem.currentSelectedGameObject.GetComponent<InventorySlot>().Ability;
        MyGuard.IsNotNull(abilityUsed, "AbilityUsed battle Ui in playerPickAbilityTurn was null.");
        abilityUsed.Apply(player, enemyYouFightin);
        ShowAbilityUsedText(this.player, abilityUsed);
        this.abilityButtonScreen.SetActive(false);
        this.backButton.SetActive(false);
        this.state = Battle.FinishedPLayerTurn;
    }

    public void OnBackButtonClicked()
    {
        buttonClickedAudioSource.Play();
        foreach (var s in ScreensNotAction)
        {
            s.SetActive(false);
        }
        this.actionButtonScreen.SetActive(true);
        backButton.SetActive(false);
        this.state = Battle.PlayerTurn;
    }
}
