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

    public void DeselectButton()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }
}