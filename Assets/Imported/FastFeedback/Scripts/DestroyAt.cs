using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAt : MonoBehaviour
{
    float time;

    void Update()
    {
        if (Time.time > time) 
        {
            Destroy(gameObject);
        }
    }

    public void Run (float time)
    {
        this.time = time;
    }
}
