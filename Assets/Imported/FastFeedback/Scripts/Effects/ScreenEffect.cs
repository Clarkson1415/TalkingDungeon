using System;
using UnityEngine;

/// <summary>
/// Applies the specified material to the camera.
/// </summary>
public class ScreenEffect : MonoBehaviour
{
    public Shader shader;
    private Material material;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader != null)
        {
            if (material == null || material.shader != shader)
            {
                material = new Material(shader);
            }
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
