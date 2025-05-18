using UnityEngine;

namespace Assets.GracesScripts.UI
{
    /// <summary>
    /// Parent class of all the book pages.
    /// </summary>
    public abstract class BookPage : MonoBehaviour
    {
        public readonly BookTab.TabType PageTabType;

        /// <summary>
        /// Call this.ToggleChildComponents() inside this method first, Then add whatever other gameobjects should be toggled with the page. 
        /// e.g. the slots in the gear page should be setActive false when the save tab is selected.
        /// </summary>
        /// <param name="OnOff"></param>
        public abstract void TogglePageComponents(bool OnOff);

        protected void ToggleChildComponents(bool OnOff)
        {
            foreach (Transform child in this.transform)
            {
                child.gameObject.SetActive(OnOff);
            }
        }
    }
}
