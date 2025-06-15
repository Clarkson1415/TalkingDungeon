using Assets.GracesScripts;
using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Is used to save the enemy to use in battle and spawn it. singleton, Instantiated in PlayerDungeon.Start() if it does not exist.
/// </summary>
public class DoStuffOnSceneLoad : MonoBehaviour
{
    [HideInInspector] public GameObject enemyWasTalkingTo;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        // called in OnEnable as the first time this guy is instantaited it also needs to restore data and OnSceneLoaded event may have already happened.
        this.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveGameUtility.Load();

        if (scene.name != TalkingDungeonScenes.Battle)
        {
            SaveGameUtility.SaveGame();
            return;
        }

        var enemyStartBattleLocation = FindObjectOfType<EnemyLocationInTurnBased>();
        MyGuard.IsNotNull(enemyStartBattleLocation);
        MyGuard.IsNotNull(enemyWasTalkingTo);
        enemyWasTalkingTo.transform.SetParent(enemyStartBattleLocation.transform);
        enemyWasTalkingTo.transform.localPosition = new Vector3(0, 0, 0);

        var battleUI = FindObjectOfType<BattleUI>();
        battleUI.SetupEnemyAfterSpawned();
    }
}
