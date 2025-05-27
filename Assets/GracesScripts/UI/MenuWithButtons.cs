using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// A class that will handle the On Pointer hover event to trigger the button highlighted sound.
    /// </summary>
    public abstract class MenuWithButtons : Menu, IPointerEnterHandler
    {
        protected GameObject lastHighlightedItem;

        /// <summary>
        /// When a raycast enabled image is highlighted with mouse.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            var button = this.GetHighlighted(eventData);
            if (button == null)
            {
                return;
            }
            button.PlayHighlightedSound();
        }

        protected DungeonButton? GetHighlighted(PointerEventData eventData)
        {
            var highlightedButton = eventData.hovered.FirstOrDefault(x => x.gameObject.transform.parent.TryGetComponent<DungeonButton>(out _));

            if (highlightedButton != null)
            {
                lastHighlightedItem = highlightedButton;

                var button = highlightedButton.GetComponentInParent<DungeonButton>();
                return button;
            }
            else
            {
                highlightedButton = eventData.hovered.FirstOrDefault(x => x.gameObject.TryGetComponent<DungeonButton>(out _));

                if (highlightedButton == null)
                {
                    Debug.Log($"{eventData.hovered} was highlighted by mouse but is not a Dungeon button");
                    return null;
                }

                lastHighlightedItem = highlightedButton;
                var button = highlightedButton.GetComponent<DungeonButton>();
                return button;
            }
        }
    }
}
