using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class ItemOptionButton : Button
{
    public Item Item;

    public void SetItem(Item item)
    {
        this.Item = item;
    }

    public void RemoveItem()
    {
        this.Item = null;
    }
}