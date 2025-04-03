using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

abstract public class Menu : MonoBehaviour
{
    [SerializeField] protected EventSystem UIEventSystem;
    [SerializeField] public Sprite emptySlotImage;


    public void Close()
    {
        this.UIEventSystem.SetSelectedGameObject(null);
    }

    public GameObject GetSelectedButton()
    {
        return this.UIEventSystem.currentSelectedGameObject;
    }

    /// <summary>
    /// returns selected. 
    /// </summary>
    public virtual Item OnButtonSelected()
    {
        var selected = this.UIEventSystem.currentSelectedGameObject;

        selected.GetComponentInChildren<TMP_Text>().text = string.Empty;
        var spriteImageComponent = selected.GetComponentInChildren<ItemOptionButtonImage>();
        spriteImageComponent.SetImage(emptySlotImage);

        this.UIEventSystem.currentSelectedGameObject.TryGetComponent<ItemOptionButton>(out var itemBut);
        var itemToReturn = itemBut.Item;

        itemBut.RemoveItem();

        return itemToReturn;
    }
}