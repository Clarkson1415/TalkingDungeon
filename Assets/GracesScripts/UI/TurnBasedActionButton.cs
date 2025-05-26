using TMPro;
using UnityEngine;

public class TurnBasedActionButton : DungeonButton
{
    public TurnBasedActions Action;

    private void Start()
    {
        var textComponent = GetComponentInChildren<TMP_Text>();

        if (textComponent.text != this.Action.ToString())
        {
            textComponent.text = this.Action.ToString();
        }
    }
}