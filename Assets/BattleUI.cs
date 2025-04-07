using Assets.GracesScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class BattleUI : MonoBehaviour
{
    private Battle state;
    PlayerDungeon player;
    private EventSystem eventSystem;
    [SerializeField] AudioSource buttonClickAudioSource;

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
        eventSystem = FindObjectOfType<EventSystem>();
        player = FindObjectOfType<PlayerDungeon>();

        state = Battle.PlayerPickActionTurn;

        eventSystem.SetSelectedGameObject(ActionButtons[0]);
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
        switch(state)
        {
            case Battle.PlayerPickActionTurn:
                if (this.actionClickedFlag)
                {
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
                        }

                        abilityButton.SetAbility(player.abilities[i]);
                    }

                    // set active the game object the turn based ability is added to
                    this.eventSystem.SetSelectedGameObject(AbilityButtonLocations[0].GetComponentInChildren<TurnBasedAbilityButton>().gameObject);

                    this.state = Battle.PlayerPickAbilityTurn;
                }
                break;
            case Battle.PlayerPickAbilityTurn:
                if (this.actionClickedFlag)
                {
                    this.state = Battle.PlayerExecuteAbility;
                }
                if (this.backButtonClicked)
                {
                    state = Battle.PlayerPickActionTurn;
                }
                break;
            case Battle.PlayerExecuteAbility:
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
