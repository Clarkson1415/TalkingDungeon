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
        this.GetComponent<TMP_Text>().text = itemName;
    }
}
