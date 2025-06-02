using Assets.GracesScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Is used to store the Enemy talking to in dialogue and instaiate enemy prefab in the battle scene. Singleton, Instantiated in PlayerDungeon.Awake().
/// </summary>
public class EnemyLoader : MonoBehaviour
{
    [HideInInspector] public GameObject enemyWasTalkingTo;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != TalkingDungeonScenes.Battle)
        {
            return;
        }

        var enemyStartBattleLocation = FindObjectOfType<EnemyLocationInTurnBased>();
        MyGuard.IsNotNull(enemyStartBattleLocation);
        enemyWasTalkingTo.transform.SetParent(enemyStartBattleLocation.transform);
        enemyWasTalkingTo.transform.localPosition = new Vector3(0, 0, 0);

        var battleUI = FindObjectOfType<BattleUI>();
        battleUI.SetupEnemyAfterSpawned();
    }
}
