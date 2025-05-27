using UnityEngine.EventSystems;
#nullable enable

namespace Assets.GracesScripts.UI
{
    public abstract class MenuWithItemSlots : MenuWithButtons, IPointerEnterHandler
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            var button = this.GetHighlighted(eventData);
            if (button is InventorySlot InvSlot)
            {
                this.UpdateItemView(InvSlot);
            }
        }

        /// <summary>
        /// When the highlighted slot it changed update item view.
        /// </summary>
        /// <param name="slot">highlighted slot.</param>
        protected abstract void UpdateItemView(InventorySlot slot);
    }
}
