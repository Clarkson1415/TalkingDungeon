using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    private Battle state;
    private PlayerDungeon player;
    [SerializeField] private GameObject WellBeingObject;

    private EventSystem evSys;
    [SerializeField] AudioSource buttonClickedAudioSource;
    [SerializeField] AudioSource buttonChangedAudioSource;

    [SerializeField] Image enemyHealthFill;

    private Enemy enemyYouFightin;

    [SerializeField] private TMP_Text enemyNameField;

    [SerializeField] private GameObject enemyHealthBarAndNameToShake;

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
        enemyYouFightin = FindObjectOfType<Enemy>();
        if (enemyYouFightin == null)
        {
            throw new ArgumentNullException("enemy cannot be null in battle scene.");
        }

        state = Battle.PlayerPickActionTurn;

        evSys.SetSelectedGameObject(ActionButtons[0]);

        this.enemyHealthFill.fillAmount = this.enemyYouFightin.currentHealth / this.enemyYouFightin.maxHealth;

        enemyNameField.text = this.enemyYouFightin.name;
    }

    private void DamageEnemy(float damage)
    {
        isEnemyTakingDamageHealthBarAnimPlaying = true;
        enemyHealthBarAndNameToShake.GetComponent<shakeObject>().StartShake(1f, 5f);
        this.enemyYouFightin.currentHealth -= damage;
        StartCoroutine(AnimateEnemyHealthLoss());
    }

    IEnumerator AnimateEnemyHealthLoss()
    {
        float damagePerSecond = 2f;

        while (this.enemyHealthFill.fillAmount > (this.enemyYouFightin.currentHealth / this.enemyYouFightin.maxHealth))
        {
            this.enemyHealthFill.fillAmount -= (damagePerSecond/100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        isEnemyTakingDamageHealthBarAnimPlaying = false;
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

    bool isEnemyTakingDamageHealthBarAnimPlaying;

    private GameObject currentSelectedButton;

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
            if(currentSelectedButton != null)
            {
                if (!this.buttonClickedAudioSource.isPlaying) // if the button clicked sound is playing don't play button changed also
                {
                    this.buttonChangedAudioSource.Play();
                }
            }

            currentSelectedButton = highlighted;
        }

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
                    Log.Print($"player used {abilityUsed.name} on {enemyYouFightin.name} for {abilityUsed.attackPower}");
                    DamageEnemy(abilityUsed.attackPower);

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
                state = Battle.EnemyTurn;
                break;
            case Battle.EnemyTurn:
                if (!isEnemyTakingDamageHealthBarAnimPlaying) // wait until enemy health bar anim finished then take enemies turn
                {
                    Debug.Log("Enemy Used slap but not really");
                    this.player.TakeDamage(15);
                    WellBeingObject.GetComponent<shakeObject>().StartShake(1f, 5f);
                    state = Battle.PlayerPickActionTurn;
                }
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
