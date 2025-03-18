using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// eg like a chest or something
/// </summary>
public class ItemContainer : MonoBehaviour, IInteracble
{
    [SerializeField] public List<Item> loot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
