using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButtonToolTip : HasATooltip
{
    private void Awake()
    {
        var ab = this.GetComponent<InventorySlot>().Ability;
        var text = $"{ab.Name}: {ab.description}";
        tmp.text = text;
    }
}
