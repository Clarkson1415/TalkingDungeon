using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Player")]
    private PlayerDungeon player;
    [SerializeField] private GameObject WellBeingObject;
    public bool PlayerEscKeyFlag;

    [Header("UI")]
    [SerializeField] AudioSource buttonClickedAudioSource;
    [SerializeField] AudioSource buttonChangedAudioSource;
    /// <summary>
    /// like attack, talk, flee etc.
    /// </summary>
    [SerializeField] private List<GameObject> ActionButtons;

    /// <summary>
    /// player abilities to use. UP TO 3 FOR NOW
    /// </summary>
    [SerializeField] private List<GameObject> AbilityButtonLocations;
    [SerializeField] private GameObject abilityButtonPrefab;

    [SerializeField] GameObject actionButtonScreen;
    [SerializeField] GameObject abilityButtonSceen;
    [SerializeField] private GameObject battleDialogueBox;

    [Header("Enemy")]
    [SerializeField] Image enemyHealthFill;
    private Unit enemyYouFightin;
    [SerializeField] private TMP_Text enemyNameField;
    [SerializeField] private GameObject enemyHealthBarAndNameToShake;

    private Battle state;
    private EventSystem evSys;

    private bool actionClickedFlag;
    private bool abilityClickedFlag;
    private bool backButtonClicked;

    private bool isDialoguePrinting;

    private IEnumerator TestDialogueBox(string text)
    {
        this.isDialoguePrinting = true;
        Debug.Log("... printing text...");
        this.battleDialogueBox.GetComponentInChildren<TMP_Text>().text = text;
        yield return new WaitForSeconds(2f);
        this.isDialoguePrinting = false;
        Debug.Log("dialogue not printing anymore");
    }

    // Start is called before the first frame update
    void Start()
    {
        evSys = FindObjectOfType<EventSystem>();
        player = FindObjectOfType<PlayerDungeon>();
        enemyYouFightin = FindObjectOfType<Unit>();
        if (enemyYouFightin == null)
        {
            throw new ArgumentNullException("enemy cannot be null in battle scene.");
        }

        state = Battle.PlayerPickActionTurn;

        evSys.SetSelectedGameObject(ActionButtons[0]);

        this.enemyHealthFill.fillAmount = this.enemyYouFightin.currentHealth / this.enemyYouFightin.maxHealth;

        enemyNameField.text = this.enemyYouFightin.name;

        // TODO do i want the dialogue box to maybe say stuff on opening, like enemy approached...
        this.battleDialogueBox.SetActive(false);
        this.abilityButtonSceen.SetActive(false);
        this.actionButtonScreen.SetActive(true);
    }

    private void DamageEnemy(float damage)
    {
        isEnemyTakingDamageHealthBarAnimPlaying = true;
        enemyHealthBarAndNameToShake.GetComponent<shakeObject>().StartShake(1f, 5f);
        this.enemyYouFightin.currentHealth -= damage;
        StartCoroutine(AnimateEnemyHealthLoss());
    }

    private IEnumerator AnimateEnemyHealthLoss()
    {
        float damagePerSecond = 2f;

        while (this.enemyHealthFill.fillAmount > (this.enemyYouFightin.currentHealth / this.enemyYouFightin.maxHealth))
        {
            this.enemyHealthFill.fillAmount -= (damagePerSecond / 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        isEnemyTakingDamageHealthBarAnimPlaying = false;
    }

    private enum Battle
    {
        PlayerPickActionTurn,
        PlayerPickAbilityTurn,
        ExecutePlayerAbilityShowText,
        EnemyTurn,
        ExecuteEnemyMoveAndPrintingText,
        PlayerWon,
        PlayerLost,
        WaitOnDeathScreen,
        Paused,
    }

    bool isEnemyTakingDamageHealthBarAnimPlaying;

    private GameObject currentSelectedButton;


    private void SetupAbilityButtons()
    {
        this.abilityButtonSceen.SetActive(true);

        // TODO setup ability buttons
        // idk if this will change mid battle otherwise can do on start in an ability UI class on the ability UI object. or in here on start would be better maybe?
        if (player.abilities.Count > 3)
        {
            throw new ArgumentException("abilities is more than supported in battle UI");
        }

        for (int i = 0; i < player.abilities.Count; i++)
        {
            var abilityButton = AbilityButtonLocations[i].GetComponentInChildren<TurnBasedAbilityButton>();

            if (abilityButton == null)
            {
                var abButtonPrefab = Instantiate(abilityButtonPrefab, AbilityButtonLocations[i].transform);
                abilityButton = abButtonPrefab.GetComponent<TurnBasedAbilityButton>();

                // TODO test this
                abilityButton.GetComponent<Button>().onClick.AddListener(OnAbilityButtonClicked);
            }

            abilityButton.SetAbility(player.abilities[i]);
        }

        // set active the game object the turn based ability is added to
        var firstButton = AbilityButtonLocations[0].GetComponentInChildren<TurnBasedAbilityButton>().gameObject;
        this.evSys.SetSelectedGameObject(firstButton);
    }

    private Battle cachedStateBeforePaused;
    [SerializeField] GameObject mainMenu;
    private Button[] ButtonsActiveBeforePaused;

    private void ToggleButtonsNotOnPauseOrDeathScreen(bool onOff)
    {
        foreach (var button in ButtonsActiveBeforePaused)
        {
            button.gameObject.SetActive(onOff);
        }
    }

    // Update is called once per frame 
    void Update()
    {
        var highlighted = this.evSys.currentSelectedGameObject;

        if (highlighted == null)
        {
            // do nothing
        }
        else if (highlighted != currentSelectedButton)
        {
            if (currentSelectedButton != null)
            {
                if (!this.buttonClickedAudioSource.isPlaying) // if the button clicked sound is playing don't play button changed also
                {
                    this.buttonChangedAudioSource.Play();
                }
            }

            currentSelectedButton = highlighted;
        }

        if (this.PlayerEscKeyFlag && state != Battle.Paused)
        {
            PlayerEscKeyFlag = false;
            cachedStateBeforePaused = this.state;
            this.state = Battle.Paused;
            // find all buttons in scene that are active then save them to reactivate when unpaused
            ButtonsActiveBeforePaused = FindObjectsByType<Button>(FindObjectsSortMode.None);
            ToggleButtonsNotOnPauseOrDeathScreen(false);
            this.mainMenu.gameObject.SetActive(true);
            this.mainMenu.GetComponent<PauseMenu>().StartPauseMenu();
        }

        switch (state)
        {
            case Battle.Paused:
                if (this.PlayerEscKeyFlag)
                {
                    PlayerEscKeyFlag = false;
                    state = cachedStateBeforePaused;
                    ToggleButtonsNotOnPauseOrDeathScreen(true);
                    this.mainMenu.GetComponent<PauseMenu>().Close();
                    this.mainMenu.gameObject.SetActive(false);
                    this.evSys.SetSelectedGameObject(this.ButtonsActiveBeforePaused[0].gameObject);
                }
                break;
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

                    switch (action)
                    {
                        case TurnBasedActions.ATTACK:
                            SetupAbilityButtons();
                            this.state = Battle.PlayerPickAbilityTurn;
                            break;
                        case TurnBasedActions.RUN:
                            // TODO idk what will actually happen if you run yet
                            QuitBattle();
                            break;
                        case TurnBasedActions.TALK:
                            // TODO 
                            StartDialogue(this.enemyYouFightin.firstDialogueSlide);
                            break;
                    }
                }
                break;
            case Battle.PlayerPickAbilityTurn:
                if (this.abilityClickedFlag)
                {
                    this.abilityClickedFlag = false;
                    var abilityUsed = evSys.currentSelectedGameObject.GetComponent<TurnBasedAbilityButton>().Ability;
                    Log.Print($"player used {abilityUsed.name} on {enemyYouFightin.name} for {abilityUsed.attackPower}");
                    DamageEnemy(abilityUsed.attackPower);

                    // TODO set dialoge box to active true
                    this.abilityButtonSceen.SetActive(false);
                    this.battleDialogueBox.SetActive(true);
                    // TODO start coroutine printdialogue
                    StartCoroutine(TestDialogueBox($"player damaged enemy for {abilityUsed.attackPower}"));
                    this.state = Battle.ExecutePlayerAbilityShowText;
                }
                if (this.backButtonClicked)
                {
                    // TODO this later when everything else working
                    this.backButtonClicked = false;
                }
                break;
            case Battle.ExecutePlayerAbilityShowText:
                // if animation finished or some delay go to action turn
                if (!isEnemyTakingDamageHealthBarAnimPlaying && !isDialoguePrinting) // wait until enemy health bar anim finished then take enemies turn
                {
                    // when finished showing player move text go to enemy move

                    // TODO this properly idk
                    this.player.TakeDamage(90);
                    Log.Print("damage taken" + 90);
                    Log.Print("current wellbeing " + this.player.currentWellbeing);

                    // TODO do i want the player to shake it in the normal scene? or only in battle idk yet 
                    // if in player it should go in player Script
                    WellBeingObject.GetComponent<shakeObject>().StartShake(1f, 5f);

                    // TODO start coroutine print dialogue
                    StartCoroutine(TestDialogueBox("hit for 15"));

                    if (this.player.currentWellbeing <= 0)
                    {
                        // TODO note this will not take into account the animation perhaps I could speed it up if the player health will be dead
                        state = Battle.PlayerLost;
                    }
                    else
                    {
                        state = Battle.ExecuteEnemyMoveAndPrintingText;
                    }
                }
                break;
            case Battle.ExecuteEnemyMoveAndPrintingText:
                if (!this.player.isHealthBarDoingAnim && !isDialoguePrinting) // when dialogue not printing and player animation finished  
                {
                    this.battleDialogueBox.SetActive(false);
                    this.actionButtonScreen.SetActive(true);
                    this.evSys.SetSelectedGameObject(ActionButtons[0]);
                    this.state = Battle.PlayerPickActionTurn;
                }
                break;
            case Battle.PlayerWon:
                break;
            case Battle.PlayerLost:
                if (!this.player.isHealthBarDoingAnim && !isDialoguePrinting) // when dialogue finished printing display death
                {
                    Debug.Log("in playerlost goto wait");
                    state = Battle.WaitOnDeathScreen;

                    // set the selected game object to the first button in the death screen and disable all other buttons not on the death screen
                    var deathscreen = FindObjectOfType<DeathScreen>();
                    var buttonsOnDeathScreen = deathscreen.gameObject.GetComponentsInChildren<Button>();

                    var allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

                    foreach (var button in allButtons)
                    {
                        if (!buttonsOnDeathScreen.Contains(button))
                        {
                            button.gameObject.SetActive(false);
                        }
                    }

                    this.evSys.SetSelectedGameObject(buttonsOnDeathScreen[0].gameObject);
                }
                break;
            case Battle.WaitOnDeathScreen:
                break;
            default:
                break;
        }
    }

    private void QuitBattle()
    {
        throw new NotImplementedException();
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
        backButtonClicked = true;
        buttonClickedAudioSource.Play();
    }
}
