using Assets.GracesScripts;
using Assets.GracesScripts.UI;
using EasyTransition;
using System;
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
    private bool IsPlayerHealthBarPlayingAnimation => player.healthBarFill != null && player.healthBarFill.fillAmount > Mathf.Clamp((player.currentHealth / player.maxHealth), 0, 1);
    private bool IsEnemyHealthAnimationPlaying => enemyYouFightin != null && enemyYouFightin.healthBarFill != null && enemyYouFightin.healthBarFill.fillAmount > Mathf.Clamp((enemyYouFightin.currentHealth / enemyYouFightin.maxHealth), 0, 1);
    private bool AreAnimationsFinished =>
    !IsPlayerHealthBarPlayingAnimation &&
    !IsEnemyHealthAnimationPlaying &&
    !battleDialogBoxAboveText.IsNotWritingOrOnSlide;

    [Header("UI")]
    [SerializeField] TransitionSettings exitBattleTransition;
    [SerializeField] AudioSource buttonClickedAudioSource;

    [SerializeField] private List<InventorySlot> AbilityButtons;
    [SerializeField] GameObject actionButtonScreen;
    [SerializeField] GameObject abilityButtonScreen;
    [SerializeField] GameObject itemScreen;
    [SerializeField] GameObject runScreen;
    [SerializeField] GameObject talkScreen;

    private BattleTextBox battleDialogBoxAboveText;

    [SerializeField] GameObject backButton;

    private List<GameObject> ScreensNotAction => new() { this.itemScreen, this.runScreen, this.abilityButtonScreen, this.talkScreen };

    [Header("Enemy")]
    private Unit_NPC? enemyYouFightin;
    [SerializeField] private TMP_Text enemyNameField;
    [SerializeField] private GameObject enemyHealthBarAndNameToShake;
    [SerializeField] private Image enemyHealthFill;

    private Battle state;
    private static Color positiveGreen = new(0, 0.7f, 0);

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
        this.battleDialogBoxAboveText.StartWriting("Your turn", Color.black);
    }

    public void SetupEnemyAfterSpawned()
    {
        enemyYouFightin = FindObjectOfType<Unit_NPC>();
        MyGuard.IsNotNull(enemyYouFightin, "enemy cannot be null in fight scene.");
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
                MyGuard.IsNotNull(enemyYouFightin, "enemyYouFightin is null.");
                if (player.currentHealth <= (enemyYouFightin.currentHealth))
                {
                    Debug.Log("Getaway based on if you have more wellbeing than enemy");
                    runSuccesss = false;
                }

                if (runSuccesss)
                {
                    this.battleDialogBoxAboveText.StartWriting("you got away!", Color.black);

                    // TODO play sound effect for running away
                    SaveGameUtility.SaveStuffFromBattle(player);
                    var scenePlayerSavedInLast = PlayerPrefs.GetString(SaveKeys.LastScene);
                    TalkingDungeonScenes.LoadScene(scenePlayerSavedInLast, exitBattleTransition, SaveGameState.BattleRunAwaySuccess);
                    this.state = Battle.WaitOnDeathScreenOrTransitioning;
                }
                else
                {
                    this.battleDialogBoxAboveText.StartWriting("Failed to get away.", Color.red);
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
                MyGuard.IsNotNull(enemyYouFightin, "enemyYouFightin is null.");
                if (enemyYouFightin.currentHealth > 0)
                {
                    this.battleDialogBoxAboveText.StartWriting("Enemy Turn", Color.black);
                    this.state = Battle.EnemyPickAbilityTurn;
                }
                else
                {
                    this.battleDialogBoxAboveText.StartWriting("You Won", Color.black);

                    var scene = enemyYouFightin.SceneAfterWin;
                    TalkingDungeonScenes.LoadScene(scene, exitBattleTransition, SaveGameState.BattleWon);
                    this.state = Battle.WaitOnDeathScreenOrTransitioning;
                }
                break;
            case Battle.EnemyPickAbilityTurn:
                // or enemy could use an item.
                Debug.Log("not finished setup here. need to calculate damage based on units current defence stat also?");
                MyGuard.IsNotNull(enemyYouFightin, "enemyYouFightin is null.");
                var enemyAbility = PickRandomAbility(this.enemyYouFightin.Abilities);
                enemyAbility.Apply(enemyYouFightin, player);
                ShowAbilityUsedText(this.enemyYouFightin, enemyAbility);
                // TODO display damage turn text on screen
                state = this.player.currentHealth <= 0 ? Battle.WaitOnDeathScreenOrTransitioning : Battle.FinishedEnemiesTurn;
                break;
            case Battle.FinishedEnemiesTurn:
                this.battleDialogBoxAboveText.StartWriting("Your Turn", Color.black);
                GotoPlayerActionTurn();
                break;
            case Battle.WaitOnDeathScreenOrTransitioning:
                break;
            default:
                break;
        }
    }

    private void ShowAbilityUsedText(DungeonUnit user, Ability abilityUsed)
    {
        var color = positiveGreen;
        var person = "Player";
        if (user is Unit_NPC)
        {
            color = Color.red;
            MyGuard.IsNotNull(enemyYouFightin, "enemyYouFightin is null.");
            person = enemyYouFightin.unitName;
        }

        var turnInfoString = $"{person} used {user.equippedWeapon.Name} to {abilityUsed.Name} to {abilityUsed.FormatDescription(user.equippedWeapon, user)}";
        this.battleDialogBoxAboveText.StartWriting(turnInfoString, color);
    }

    private static readonly System.Random Random = new();

    private Ability PickRandomAbility(List<Ability> abilities)
    {
        MyGuard.IsNotNull(enemyYouFightin, "enemyYouFightin is null.");
        var abilityIndex = Random.Next(0, this.enemyYouFightin.Abilities.Count);
        return abilities[abilityIndex];
    }

    /// <summary>
    /// To Start the Talk Option in the battle.
    /// </summary>
    /// <param name="dialogue"></param>
    /// <exception cref="NotImplementedException"></exception>
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

    public void GotoPlayerActionTurn()
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
