using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// A class that will handle the On Pointer hover event to trigger the button highlighted sound.
    /// </summary>
    public class MenuWithButtons : Menu, IPointerEnterHandler
    {
        protected GameObject lastHighlightedItem;

        /// <summary>
        /// When a raycast enabled image is highlighted with mouse.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.OnSlotChangePlaySound(eventData);
        }

        /// <summary>
        /// Only for if player is using mouse controls
        /// </summary>
        /// <param name="eventData"></param>
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

            this.UIEventSystem.SetSelectedGameObject(highlightedButton);
            lastHighlightedItem = highlightedButton;

            var button = highlightedButton.GetComponentInParent<DungeonButton>();
            button.PlayHighlightedSound();
        }
    }
}
