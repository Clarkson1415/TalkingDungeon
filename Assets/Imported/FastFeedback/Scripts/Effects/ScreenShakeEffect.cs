using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a simple transform-offset screen shake.
/// </summary>
public class ScreenShakeEffect : MonoBehaviour
{
    private Vector3 cameraPosition;

    public float shakeStrength = 0.0f;

    /// <summary>
    /// Update the strength of the camera shake every frame.
    /// </summary>
    void Update()
    {
        shakeStrength = shakeStrength * (1.0f - shakeStrength * Time.deltaTime * 10.0f);
        shakeStrength = Mathf.Max(0, shakeStrength - Time.deltaTime * 0.3f);
    }

    /// <summary>
    /// Stores the position before the screen shake, then offsets the camera position.
    /// </summary>
    private void LateUpdate()
    {
        cameraPosition = transform.position;
        transform.position += 
            transform.up * Random.Range(-shakeStrength, shakeStrength) + 
            transform.right * Random.Range(-shakeStrength, shakeStrength);
    }

    /// <summary>
    /// After the camera is rendered, move the camera back so that it is in the correct
    /// position if other scripts need to read/adjust it.
    /// </summary>
    private void OnPostRender()
    {
        transform.position = cameraPosition;
    }
}
