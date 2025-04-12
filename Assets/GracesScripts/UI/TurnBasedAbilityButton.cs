using Assets.GracesScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedAbilityButton : MonoBehaviour
{
    public Ability Ability;

    private void Start()
    {
        //if(abilityButton.Tr<Button>().onClick.AddListener(OnAbilityButtonClicked);)
        if(this.TryGetComponent<Button>(out var butt))
        {
            Log.Print($"reminder: onclick events are set in script not in inspector currently: {butt.onClick}");
        }
    }

    public void SetAbility(Ability ablity)
    {
        this.Ability = ablity;

        var text = this.GetComponentInChildren<TMP_Text>();
        text.text = ablity.Name;
    }
}