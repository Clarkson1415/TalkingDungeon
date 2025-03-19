using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


/// <summary>
/// Button class, draws itself upon activation / instantiation.
/// </summary>
public class Button : MonoBehaviour
{
    public bool isSelected;

    public string OptionText { get; set; }

    public void Start()
    {
    }

    public void UpdateButtonText()
    {
        this.GetComponentInChildren<TMP_Text>().text = this.OptionText;
    }

    /// <summary>
    /// is used for something in the UI idk where
    /// </summary>
    public void ClickButton()
    {
        isSelected = true;
    }
}