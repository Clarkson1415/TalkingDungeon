using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpriteContainer : MonoBehaviour
{
    public void SetItem(Sprite image)
    {
        this.GetComponent<Image>().sprite = image;
    }
}
