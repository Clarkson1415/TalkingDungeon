using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// eg like a chest or something
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class ItemContainer : MonoBehaviour, IInteracble
{
    /// <summary>
    /// Loot chest starts out with, after it's been saved, loot is then loaded from player prefs
    /// </summary>
    public List<DungeonItem> Loot;

    [SerializeField] private AudioClip chestOpenSound;
    [SerializeField] private AudioClip chestClosedSound;
    private AudioSource AudioSource;
    private Guid chestID;

    private void Awake()
    {
        AudioSource = this.GetComponent<AudioSource>();
    }

    public void SaveContents()
    {
        Debug.Log("TODO call this on all chests to save chests in saved scene. Then each chest could load itself when load? idk ");
    }

    public void PlayOpenSound()
    {
        this.AudioSource.clip = chestOpenSound;
        this.AudioSource.Play();
    }

    public void PlayClosedSound()
    {
        this.AudioSource.clip = chestClosedSound;
        this.AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
