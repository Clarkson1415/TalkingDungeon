using UnityEngine;
using UnityEngine.EventSystems;

abstract public class Menu : MonoBehaviour
{
    [SerializeField] protected EventSystem UIEventSystem;
    [SerializeField] public Sprite emptySlotImage;

    public virtual void Close()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }

    public GameObject GetSelectedButton()
    {
        return this.UIEventSystem.currentSelectedGameObject;
    }
}