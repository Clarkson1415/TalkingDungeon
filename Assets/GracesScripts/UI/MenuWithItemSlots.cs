using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable

namespace Assets.GracesScripts.UI
{
    public abstract class MenuWithItemSlots : Menu, IPointerEnterHandler
    {
        private GameObject lastHighlightedItem;

        /// <summary>
        /// When a raycast enabled image is highlighted with mouse.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.OnSlotChangePlaySound(eventData);
        }

        private void OnSlotChangePlaySound(PointerEventData eventData)
        {
            var highlightedButton = eventData.hovered.FirstOrDefault(x => x.gameObject.transform.parent.TryGetComponent<DungeonButton>(out _));

            if (highlightedButton == lastHighlightedItem)
            {
                return;
            }

            if (highlightedButton == null)
            {
                return;
            }

            lastHighlightedItem = highlightedButton;

            var button = highlightedButton.GetComponentInParent<DungeonButton>();
            button.PlayHighlightedSound();

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
