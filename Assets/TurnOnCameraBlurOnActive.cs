using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TurnOnCameraBlurOnActive : MonoBehaviour
{
    private DepthOfField depthBlur;

    private void OnEnable()
    {
        if (depthBlur == null)
        {
            var Cam = FindObjectOfType<Camera>();
            var volume = Cam.gameObject.GetComponent<Volume>();
            volume.profile.TryGet(out depthBlur);
        }

        depthBlur.active = true;
    }

    private void OnDisable()
    {
        if (depthBlur == null)
        {
            var Cam = FindObjectOfType<Camera>();
            var volume = Cam.gameObject.GetComponent<Volume>();
            volume.profile.TryGet(out depthBlur);
        }

        depthBlur.active = false;
    }
}
