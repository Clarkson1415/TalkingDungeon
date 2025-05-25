using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.GracesScripts.UI
{
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] protected EventSystem UIEventSystem;

        public virtual void Close()
        {
            this.UIEventSystem.SetSelectedGameObject(null);
            this.gameObject.SetActive(false);
        }

        public GameObject GetSelectedButton()
        {
            var selected = this.UIEventSystem.currentSelectedGameObject;
            this.UIEventSystem.SetSelectedGameObject(null);
            return selected;
        }
    }
}