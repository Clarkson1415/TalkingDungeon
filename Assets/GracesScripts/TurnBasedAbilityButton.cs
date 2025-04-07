using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedAbilityButton : MonoBehaviour
{
    public Ability Ability;

    public void SetAbility(Ability ablity)
    {
        this.Ability = ablity;

        var text = this.GetComponentInChildren<TMP_Text>();
        text.text = ablity.Name;
    }
}