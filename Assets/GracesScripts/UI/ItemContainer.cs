using Assets.GracesScripts.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// eg like a chest or something
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ItemContainer : MonoBehaviour, IInteracble, ISaveable
{
    public string UniqueId => this.name;

    public object CaptureState()
    {
        var data = new ItemContainerData()
        {
            Loot = this.Loot,
            animatorStateName = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name,
        };

        return data;
    }

    public void RestoreState(object state)
    {
        ItemContainerData data = (ItemContainerData)state;
        this.Loot = data.Loot;
        this.animator.Play(data.animatorStateName);
    }

    /// <summary>
    /// Loot chest starts out with, after it's been saved, loot is then loaded from player prefs
    /// </summary>
    public List<DungeonItem> Loot;

    [SerializeField] private AudioClip chestOpenSound;
    [SerializeField] private AudioClip chestClosedSound;
    private AudioSource AudioSource;
    private Guid chestID;
    private Animator animator;
    private ContainerMenu ContainerMenu;
    private bool _finishedInteraction;
    public bool FinishedInteraction { get => _finishedInteraction; set => _finishedInteraction = value; }

    private void Awake()
    {
        AudioSource = this.GetComponent<AudioSource>();
        this.animator = this.GetComponent<Animator>();

        var menuReferences = FindObjectOfType<MenuReferences>();
        this.ContainerMenu = menuReferences.containerMenu;
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

    public virtual void Interact()
    {
        _finishedInteraction = false;
        this.ContainerMenu.gameObject.SetActive(true);
        this.animator.SetTrigger("Opened");
        this.PlayOpenSound();
        this.ContainerMenu.SetupMenu(this);
    }

    public virtual void EndInteract()
    {
        this.PlayClosedSound();
        _finishedInteraction = true;
    }
}
