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
        var saveData = FindObjectOfType<PersistanctSaveData>();
        saveData.SavePlayerData();

        // using transition package
        TransitionManager.Instance().Transition(nextScene, transition, 0f);
    }
}
