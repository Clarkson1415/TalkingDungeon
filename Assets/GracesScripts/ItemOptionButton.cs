using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemOptionButton : Button
{
    public Item Item;
    [SerializeField] Sprite equippedOverlayGraphic;
    [SerializeField] Sprite unequippedGraphic;

    [SerializeField] Image EquippedOverlayTargetImage;

    public void SetItemAndImage(Item item)
    {
        this.Item = item;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(item.image);
    }

    public void RemoveItem()
    {
        this.Item = null;
    }

    /// <summary>
    /// Item Option Button applies equipped overlay to it.
    /// </summary>
    public override void ClickButton()
    {
        base.ClickButton(); 
    }

    public void ToggleEquipGraphic()
    {
        if (EquippedOverlayTargetImage.sprite != equippedOverlayGraphic)
        {
            EquippedOverlayTargetImage.sprite = equippedOverlayGraphic;
        }
        else
        {
            EquippedOverlayTargetImage.sprite = unequippedGraphic;
        }
    }

}