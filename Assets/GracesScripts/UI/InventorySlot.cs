using Assets.GracesScripts.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
#nullable enable

public class InventorySlot : DungeonButton
{
    public DungeonItem? Item;
    public Ability? Ability;

    [SerializeField] Sprite equippedOverlayGraphic;
    [SerializeField] Sprite unequippedGraphic;
    [SerializeField] Image EquippedOverlayTargetImage;
    public void SetItemAndImage(DungeonItem? item)
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

    public void SetAbilityAndImage(Ability? ability, Weapon weapon)
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
        this.gameObject.GetComponent<AbilityButtonToolTip>().UpdateAbilityToolTip(weapon);
    }

    public void ReplaceSlotWithBlanks()
    {
        this.Item = null;
        this.Ability = null;

        var spriteImageComponent = this.gameObject.GetComponentInChildren<InventorySlotImage>();
        spriteImageComponent.SetImage(unequippedGraphic);
    }

    public override void PlaySelectSound()
    {
        base.PlaySelectSound();
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