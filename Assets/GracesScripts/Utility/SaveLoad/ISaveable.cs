using UnityEngine;

public interface ISaveable
{
    string UniqueId { get; }  // Ideally a stable ID
    object CaptureState();    // return savedata object
    void RestoreState(string json); // Applies loaded data
}
