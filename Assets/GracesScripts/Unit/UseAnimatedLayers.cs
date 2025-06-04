using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// attach to Game object with layers of animation to line up. Kinda like an interface but cant do getComponent in Awake in abstract classes or interfaces.
/// </summary>
public class UseAnimatedLayers : MonoBehaviour
{
    private List<Animator> animators = new();

    private void Awake()
    {
        animators.AddRange(this.GetComponentsInChildren<Animator>().ToList()); // this also gets the Player Animator IDK how but it does.
    }

    public void SetBools(string paramName, bool value)
    {
        foreach (var anim in this.animators)
        {
            anim.SetBool(paramName, value);
        }
    }

    public void SetTriggers(string paramName)
    {
        foreach (var anim in this.animators)
        {
            anim.SetTrigger(paramName);
        }
    }

    public void SetFloats(string paramName, float value)
    {
        foreach (var anim in this.animators)
        {
            anim.SetFloat(paramName, value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
