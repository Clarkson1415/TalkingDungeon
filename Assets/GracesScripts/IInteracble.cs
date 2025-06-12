using UnityEngine;

public interface IInteracble
{
    public abstract void Interact();
    public abstract void EndInteract();

    [HideInInspector] public bool FinishedInteraction { get; set; }
}
