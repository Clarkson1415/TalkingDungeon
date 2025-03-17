using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class do not add to unity component
/// </summary>
public class UsesProfilePic : MonoBehaviour
{
    [SerializeField] protected Sprite profilePic;
    public Sprite ProfilePic { get => this.profilePic; set => profilePic = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: behaviour??? idk they don't have any yet, if none make this an INPC
    }
}
