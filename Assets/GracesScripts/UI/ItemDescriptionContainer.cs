using TMPro;
using UnityEngine;

public class ItemDescriptionContainer : MonoBehaviour
{
    public void SetDescription(string description)
    {
        var textComponent = this.GetComponentInChildren<TMP_Text>();

        if (textComponent == null)
        {
            Debug.LogError($"could not find text component in children of {this.name}, setting description {description}");
        }

        textComponent.text = description;
    }
}