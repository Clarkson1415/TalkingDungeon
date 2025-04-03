using Assets.GracesScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ButtonMenuOption : Button
{
    [SerializeField] string buttonText;

    private void Awake()
    {
        this.GetComponentInChildren<TMP_Text>().text = this.buttonText;
    }

    public override void ClickButton()
    {
        base.ClickButton();

        // and also do the menu action.
        Log.Print($"menu action todo: {this.buttonText}");
    }
}