using Assets.GracesScripts;
using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadNextScene : MonoBehaviour
{
    public TransitionSettings transition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Log.Print("Scenes");

        List<string> scenes = new();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
                Log.Print(scene.path);
            }
        }

        var secondS = scenes.FirstOrDefault(x => x.Contains("Dungeon2"));
        // using transition package
        TransitionManager.Instance().Transition(secondS, transition, 0f);

        // normal unity scene transition
        // SceneManager.LoadScene(secondS);
    }
}
