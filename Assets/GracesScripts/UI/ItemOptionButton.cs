using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#nullable enable

public class ItemOptionButton : DungeonButton
{
    public Item? Item;
    [SerializeField] Sprite equippedOverlayGraphic;
    [SerializeField] Sprite unequippedGraphic;
    [SerializeField] Image EquippedOverlayTargetImage;
    public void SetItemAndImage(Item? item)
    {
        if(item == null)
        {
            ReplaceItemWithBlank();
            return;
        }

        this.Item = item;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(item.image);
    }

    public void ReplaceItemWithBlank()
    {
        this.Item = null;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(unequippedGraphic);
    }

    /// <summary>
    /// Item Option Button applies equipped overlay to it.
    /// </summary>
    public override void ClickButton()
    {
        Debug.Log("On Click");

        if (this.Item != null)
        {
            base.ClickButton();
        }
    }

    public void ToggleEquipGraphic(bool isEquipped)
    {
        if (isEquipped)
        {
            EquippedOverlayTargetImage.sprite = equippedOverlayGraphic;
        }
        else
        {
            EquippedOverlayTargetImage.sprite = unequippedGraphic;

        }
    }

}