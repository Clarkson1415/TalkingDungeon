using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedActionButton : MonoBehaviour
{
    public TurnBasedActions Action;

    private void Start()
    {
        var textComponent = GetComponentInChildren<TMP_Text>();

        if(textComponent.text != this.Action.ToString())
        {
            textComponent.text = this.Action.ToString();
        }
    }
}