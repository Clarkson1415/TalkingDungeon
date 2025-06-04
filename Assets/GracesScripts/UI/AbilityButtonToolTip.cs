using Assets.GracesScripts.ScriptableObjects;

public class AbilityButtonToolTip : HasATooltip
{
    public void UpdateAbilityToolTip(Weapon weapon)
    {
        var initialSteate = this.gameObject.activeSelf;
        this.toolTip.SetActive(true);
        this.gameObject.SetActive(true);
        var ab = this.GetComponent<InventorySlot>().Ability;
        var text = "Empty Ability Slot";

        var player = FindObjectOfType<PlayerDungeon>();
        MyGuard.IsNotNull("Player not found.");

        if (ab != null)
        {
            text = $"{ab.FormatDescription(weapon, player)}";
        }

        tmp.text = text;
        this.gameObject.SetActive(initialSteate);
        this.toolTip.SetActive(false);
    }
}
