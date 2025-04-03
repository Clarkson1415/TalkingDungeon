using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ItemOptionButton : Button
{
    public Item Item;

    public void SetItem(Item item)
    {
        this.Item = item;
    }
}