using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Is used to store the Enemy talking to in dialogue and instaiate enemy prefab in the battle scene. Singleton, Instantiated in PlayerDungeon.Awake().
/// </summary>
public class EnemyLoader : MonoBehaviour
{
    [HideInInspector] public GameObject enemyStartingBattleWithsPrefab;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GameScene.Battle)
        {
            return;
        }

        var enemyStartBattleLocation = FindObjectOfType<EnemyLocationInTurnBased>();
        Instantiate(enemyStartingBattleWithsPrefab, enemyStartBattleLocation.transform);

        var battleUI = FindObjectOfType<BattleUI>();
        battleUI.SetupEnemyAfterSpawned();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
