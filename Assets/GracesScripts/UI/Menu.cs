using UnityEngine;
using UnityEngine.EventSystems;

abstract public class Menu : MonoBehaviour
{
    [SerializeField] protected EventSystem UIEventSystem;
    [SerializeField] public Sprite emptySlotImage;

    public virtual void Close()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
        this.gameObject.SetActive(false);
    }

    public GameObject GetSelectedButton()
    {
        return this.UIEventSystem.currentSelectedGameObject;
    }

    /// <summary>
    /// remove from UI selected. probably also reverts the button graphic if there is one for selected idk.
    /// </summary>
    public void DeselectButton()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }
}