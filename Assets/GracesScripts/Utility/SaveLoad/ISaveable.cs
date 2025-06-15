using UnityEngine;

public interface ISaveable
{
    string UniqueId { get; }  // Ideally a stable ID
    object CaptureState();    // Saves data to Player Prefs
    void RestoreState(object state); // Applies loaded data
}
