using UnityEngine;
using UnityEngine.UI;
#nullable enable

public class InventorySlot : DungeonButton
{
    public Item? Item;
    public Ability? Ability;

    [SerializeField] Sprite equippedOverlayGraphic;
    [SerializeField] Sprite unequippedGraphic;
    [SerializeField] Image EquippedOverlayTargetImage;
    public void SetItemAndImage(Item? item)
    {
        if (item == null)
        {
            ReplaceSlotWithBlanks();
            return;
        }

        this.Item = item;
        this.Ability = null;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<InventorySlotImage>();
        spriteImageComponent.SetImage(item.image);
    }

    /// <summary>
    /// Should SPlit ITem into ability and item items classes as this is only for abilities.
    /// </summary>
    /// <param name="text"></param>
    public void UpdateToolTip(string text)
    {
        var toolTip = this.GetComponentInParent<HasATooltip>();
        MyGuard.IsNotNull(toolTip, "No tooltip founnd in parent");
        toolTip.ChangeToolTipText(text);
    }

    public void SetAbilityAndImage(Ability? ability)
    {
        if (ability == null)
        {
            ReplaceSlotWithBlanks();
            return;
        }

        this.Ability = ability;
        this.Item = null;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<InventorySlotImage>();
        spriteImageComponent.SetImage(Ability.image);
    }

    public void ReplaceSlotWithBlanks()
    {
        this.Item = null;
        this.Ability = null;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<InventorySlotImage>();
        spriteImageComponent.SetImage(unequippedGraphic);
    }

    /// <summary>
    /// Item Option Button applies equipped overlay to it.
    /// </summary>
    public override void PlaySelectSound()
    {
        Debug.Log("On Click");

        if (this.Item != null)
        {
            base.PlaySelectSound();
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