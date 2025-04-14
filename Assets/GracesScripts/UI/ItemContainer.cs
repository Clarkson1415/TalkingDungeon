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
    public List<Item> Loot;

    [SerializeField] private AudioClip chestOpenSound;
    [SerializeField] private AudioClip chestClosedSound;
    private AudioSource AudioSource;
    private Guid chestID;

    private void Awake()
    {
        AudioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // check all chests in scene have unique Names



        // if player prefs get data for this chest is null then use firstTimeLoot otherwise use the saved data for loot

        //if (PlayerPrefs.GetString())
    }

    public void SaveContents()
    {
        //PlayerPrefs.SetString(this.name, );
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
