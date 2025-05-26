using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a class that will verify that i do not have duplicate instances of Items in the scene. Each should be unique.
/// </summary>
public class DuplicateItemChecker : MonoBehaviour
{
    void Start()
    {
        var player = FindObjectOfType<PlayerDungeon>();

        List<DungeonItem> FoundItems = new();

        foreach (var item in player.Inventory)
        {
            FoundItems.Add(item);
        }

        List<ItemContainer> containersInScene = new();

        foreach (var container in containersInScene)
        {
            var items = container.Loot.Select(x => x).ToList();
            
            foreach(var i in items)
            {
                if (FoundItems.Contains(i))
                {
                    throw new ArgumentException($"you cannot have duplicate items in the game. duplicate found in {container.name} at {container.transform.position}");
                }
                else
                {
                    FoundItems.Add(i);
                }
            }
        }
    }
}
