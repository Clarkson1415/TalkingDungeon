using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class OrderLayersOnStairs : MonoBehaviour
{
    [SerializeField] PlayerDungeon player;
    /// <summary>
    /// the rb and box collider that stops player going through stairs when in lower layer
    /// </summary>
    [SerializeField] GameObject inStairBlock;

    /// <summary>
    /// the rb and box collider that stops playre going through stiars from the back not the entrance of stairs.
    /// </summary>
    [SerializeField] GameObject AboveStairBlock;


    [SerializeField] GameObject TriggerNewScene;

    // Trigger entry game object is only enabled when player on stairs and if treggered loads next  else if player walks out its disabled.

    // Tile map and player are on layer: Player&PlayerLevelSprites
    // tilemap starts at ordre = -1
    // player order = 0

    // when plaer in goto layer -2
    // and turn of shadow caster



    // Start is called before the first frame update
    void Start()
    {
        inStairBlock.SetActive(false);
        inStairBlock.SetActive(true);
        TriggerNewScene.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // swap order in layer of Ground vs player to be ground +1 above player
        player.GetComponent<ShadowCaster2D>().enabled = false;
        player.GetComponent<SortingGroup>().sortingOrder = -2;
        inStairBlock.SetActive(true);
        AboveStairBlock.SetActive(false);
        TriggerNewScene.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(player == null || inStairBlock == null || AboveStairBlock == null || TriggerNewScene == null)
        {
            return;
        }

        // swap order so ground is -1 from player 
        player.GetComponent<ShadowCaster2D>().enabled = true;
        player.GetComponent<SortingGroup>().sortingOrder = 0;
        inStairBlock.SetActive(false);
        AboveStairBlock.SetActive(true);
        TriggerNewScene.SetActive(false);
    }
}
