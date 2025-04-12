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
    [SerializeField] private string nextScene;

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var saveData = FindFirstObjectByType<PersistantSaveData>();
        saveData.SavePlayerData();
        saveData.scenesGoneThrough += 1;

        // using transition package
        TransitionManager.Instance().Transition(nextScene, transition, 0f);
    }
}
