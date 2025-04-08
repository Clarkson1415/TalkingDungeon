using UnityEngine;
using UnityEngine.UI;

public class ItemOptionButton : DungeonButton
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