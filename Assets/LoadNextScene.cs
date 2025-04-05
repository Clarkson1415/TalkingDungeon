using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
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

        SceneManager.LoadScene(secondS);
    }
}
