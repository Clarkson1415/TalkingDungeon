using TMPro;
using UnityEngine;
#nullable enable

/// <summary>
/// Button class, draws itself upon activation / instantiation.
/// </summary>
public class Button : MonoBehaviour
{
    [SerializeField] AudioClip highlightSound;
    [SerializeField] AudioClip? selectedSound;
    [SerializeField] AudioSource audioSource;

    public bool isSelected;

    public void PlayHighlightOptionChangedSound()
    {
        if (this.audioSource.clip != highlightSound)
        {
            this.audioSource.clip = highlightSound;
        }

        this.audioSource.Play();
    }

    /// <summary>
    /// Plays the selected sound.
    /// </summary>
    public virtual void ClickButton()
    {
        isSelected = true;

        if (selectedSound == null)
        {
            // dialogue buttons wont have a selected sound;
            return; 
        }

        if (this.audioSource.clip != selectedSound)
        {
            this.audioSource.clip = selectedSound;
        }

        this.audioSource.Play();
    }
}