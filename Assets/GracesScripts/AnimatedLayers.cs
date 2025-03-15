using Assets.GracesScripts;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

/// <summary>
/// For Game object with layers of animation to line up.
/// </summary>
public class AnimatedLayers : MonoBehaviour
{
    private List<Animator> animators = new();

    private void Awake()
    {
        animators.AddRange(this.GetComponentsInChildren<Animator>().ToList()); // this also gets the Player Animator IDK how but it does.
        
        // TODO: MAKE SURE THIS IS GETTING ALL THE ANIMTORS INCLUDING in the CHILDRENS. i.e. clothing and hair etc.
        Log.Print(animators.Count.ToString());

        // then wrap the function that change the animator in a function from this class, to change all of them together.
    }

    public void SetBools(string paramName, bool value)
    {
        foreach(var anim in this.animators)
        {
            anim.SetBool(paramName, value);
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
