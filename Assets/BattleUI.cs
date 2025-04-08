using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private Battle state;
    PlayerDungeon player;
    private EventSystem evSys;
    [SerializeField] AudioSource buttonClickAudioSource;
    [SerializeField] Image enemyHealthFill;
    private Enemy enemy;

    /// <summary>
    /// like attack, talk, flee etc.
    /// </summary>
    [SerializeField] private List<GameObject> ActionButtons;

    /// <summary>
    /// player abilities to use. UP TO 3 FOR NOW
    /// </summary>
    [SerializeField] private List<GameObject> AbilityButtonLocations;

    [SerializeField] private GameObject abilityButtonPrefab;

    private bool actionClickedFlag;
    private bool abilityClickedFlag;
    private bool backButtonClicked;

    [SerializeField] GameObject actionButtonScreen;
    [SerializeField] GameObject abilityButtonSceen;

    // Start is called before the first frame update
    void Start()
    {
        evSys = FindObjectOfType<EventSystem>();
        player = FindObjectOfType<PlayerDungeon>();
        enemy = FindObjectOfType<Enemy>();
        if (enemy == null)
        {
            throw new ArgumentNullException("enemy cannot be null in battle scene.");
        }

        state = Battle.PlayerPickActionTurn;

        evSys.SetSelectedGameObject(ActionButtons[0]);

        this.enemyHealthFill.fillAmount = this.enemy.currentHealth / this.enemy.maxHealth;
    }

    private enum Battle
    {
        PlayerPickActionTurn,
        PlayerPickAbilityTurn,
        PlayerExecuteAbility,
        EnemyTurn,
        EnemyExecuteAction,
        PlayerWon,
        PlayerLost,
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case Battle.PlayerPickActionTurn:
                if (this.actionClickedFlag)
                {
                    this.actionClickedFlag = false;

                    this.abilityButtonSceen.SetActive(true);
                    this.actionButtonScreen.SetActive(false);

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
                    this.evSys.SetSelectedGameObject(AbilityButtonLocations[0].GetComponentInChildren<TurnBasedAbilityButton>().gameObject);

                    this.state = Battle.PlayerPickAbilityTurn;
                }
                break;
            case Battle.PlayerPickAbilityTurn:
                if (this.abilityClickedFlag)
                {
                    this.abilityClickedFlag = false;
                    var abilityUsed = evSys.currentSelectedGameObject.GetComponent<TurnBasedAbilityButton>().Ability;
                    Log.Print($"player used {abilityUsed.name} on {enemy.name} for {abilityUsed.attackPower}");
                    this.enemy.currentHealth -= abilityUsed.attackPower;
                    this.enemyHealthFill.fillAmount = this.enemy.currentHealth / this.enemy.maxHealth;
                    this.state = Battle.PlayerExecuteAbility;
                }
                if (this.backButtonClicked)
                {
                    this.backButtonClicked = false;
                    state = Battle.PlayerPickActionTurn;
                }
                break;
            case Battle.PlayerExecuteAbility:
                // if animation finished or some delay go to action turn
                this.abilityButtonSceen.SetActive(false);
                this.actionButtonScreen.SetActive(true);
                this.evSys.SetSelectedGameObject(ActionButtons[0]);
                state = Battle.PlayerPickActionTurn;
                break;
            case Battle.EnemyTurn:
                break;
            case Battle.EnemyExecuteAction:
                break;
            case Battle.PlayerWon:
                break;
            case Battle.PlayerLost:
                break;
        }
    }

    public void OnActionButtonClicked()
    {
        this.actionClickedFlag = true;
        buttonClickAudioSource.Play();
    }

    public void OnAbilityButtonClicked()
    {
        this.abilityClickedFlag = true;
        buttonClickAudioSource.Play();
    }

    public void OnBackButtonClicked()
    {
        backButtonClicked = true;
        buttonClickAudioSource.Play();
    }
}
