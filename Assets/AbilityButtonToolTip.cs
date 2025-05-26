public class AbilityButtonToolTip : HasATooltip
{

    private void Awake()
    {
        UpdateAbilityToolTip();
    }

    public void UpdateAbilityToolTip()
    {
        var initialSteate = this.gameObject.activeSelf;
        this.toolTip.SetActive(true);
        this.gameObject.SetActive(true);
        var ab = this.GetComponent<InventorySlot>().Ability;
        var text = "Empty Ability Slot";
        if (ab != null)
        {
            text = $"{ab.Name}: {ab.description}";
        }
        tmp.text = text;
        this.gameObject.SetActive(initialSteate);
        this.toolTip.SetActive(false);
    }
}
