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
}