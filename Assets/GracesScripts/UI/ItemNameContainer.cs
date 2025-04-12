using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNameContainer : MonoBehaviour
{
    public void SetName(string itemName)
    {
        var nameText = this.GetComponentInChildren<TMP_Text>();
        if (nameText == null)
        {
            Debug.LogError($"did not find name text {this.name} setting itemname {itemName}");
        }

        nameText.text = itemName;
    }
}
