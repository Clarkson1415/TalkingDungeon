using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackTest : MonoBehaviour
{

    public GameFeedback levelLostFeedback;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("screenshake");
        levelLostFeedback?.ActivateFeedback(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
