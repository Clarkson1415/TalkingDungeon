using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// eg like a chest or something
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class ItemContainer : MonoBehaviour, IInteracble
{
    [SerializeField] public List<Item> loot;
    [SerializeField] private AudioClip chestOpenSound;
    [SerializeField] private AudioClip chestClosedSound;
    private AudioSource AudioSource;

    private void Awake()
    {
        AudioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
