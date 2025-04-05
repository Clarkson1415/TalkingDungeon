using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFeedback;
using UnityEngine.UI;

public class FlashTextureEffect : FastFeedbackEffect
{
    [HideInInspector] public FlashColor settings;

    private Material material;

    private float startTime;
    private float finishTime;

    public Renderer[] renderers;
    public List<Material> materials = new List<Material>();
    public List<Color> tints = new List<Color>();
    public AnimationCurve curve;
    List<bool> isEmissionEnabled = new List<bool>();

    public List<Color> emissionColors = new List<Color>();

    public bool flashWithEmission = false;

    private List<MaterialGlobalIlluminationFlags> giEmissionFlags = new List<MaterialGlobalIlluminationFlags>();
    private Color flashColor;

    public void Setup (Color color, float duration)
    {
        flashWithEmission = true;
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        if (animator == null)
            renderers = gameObject.GetComponentsInChildren<Renderer>();
        else
        {
            renderers = animator.gameObject.GetComponentsInChildren<Renderer>();
        }

        materials = new List<Material>();
        isEmissionEnabled = new List<bool>();
        tints = new List<Color>();
        emissionColors = new List<Color>();
        giEmissionFlags = new List<MaterialGlobalIlluminationFlags>();

        materials.Clear();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                //Debug.Log(material.shader.name);
                if (material.shader.name != "Custom/Outline Mask" && material.shader.name != "Custom/Outline Fill")
                {

                    materials.Add(material);
                    if (material.HasColor("_Color"))
                        tints.Add(material.GetColor("_Color"));

                    if (flashWithEmission)
                    {
                        if (material.HasColor("_EmissionColor"))
                            emissionColors.Add(material.GetColor("_EmissionColor"));
                        giEmissionFlags.Add(material.globalIlluminationFlags);

                        isEmissionEnabled.Add(material.IsKeywordEnabled("_EMISSION"));
                        material.EnableKeyword("_EMISSION");
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        material.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        startTime = Time.time;
        finishTime = Time.time + duration;
        flashColor = color;
        //curve = GameManager.assets.smoothInOutCurve;
    }

    public override void Apply(FeedbackItem item, GameObject target = null, GameObject origin = null)
    {
        base.Apply(item, target, origin);
        settings = (FlashColor)item;
        flashWithEmission = settings.flashUsingEmission;
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        if (animator == null)
            renderers = gameObject.GetComponentsInChildren<Renderer>();
        else
        {
            renderers = animator.gameObject.GetComponentsInChildren<Renderer>();
        }

        materials = new List<Material>();
        isEmissionEnabled = new List<bool>();
        tints = new List<Color>();
        emissionColors = new List<Color>();
        giEmissionFlags = new List<MaterialGlobalIlluminationFlags>();

        materials.Clear();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                //Debug.Log(material.shader.name);
                if (material.shader.name != "Custom/Outline Mask" && material.shader.name != "Custom/Outline Fill")
                {
                    
                    materials.Add(material);
                    if (material.HasColor("_Color"))
                        tints.Add(material.GetColor("_Color"));

                    if (flashWithEmission)
                    {
                        if (material.HasColor("_EmissionColor"))
                            emissionColors.Add(material.GetColor("_EmissionColor"));
                        giEmissionFlags.Add(material.globalIlluminationFlags);

                        isEmissionEnabled.Add(material.IsKeywordEnabled("_EMISSION"));
                        material.EnableKeyword("_EMISSION");
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;

                        material.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }
        startTime = Time.time;
        finishTime = Time.time + settings.flashTime;
        curve = settings.GetAnimationCurve();
        flashColor = settings.flashColor;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            
            if (materials[i].HasColor("_Color"))
                materials[i].SetColor("_Color", tints[i]);

            if (flashWithEmission)
            {
                if (materials[i].HasColor("_EmissionColor") && emissionColors.Count > i)
                    materials[i].SetColor("_EmissionColor", emissionColors[i]);

                if (giEmissionFlags.Count > i)
                    materials[i].globalIlluminationFlags = giEmissionFlags[i];

                if (isEmissionEnabled.Count > i && !isEmissionEnabled[i])
                    materials[i].DisableKeyword("_EMISSION");

            }
        }
    }

    private void Update()
    {
        float mod = curve.Evaluate((Time.time - startTime) / (finishTime - startTime));

        if (flashWithEmission)
        {
            for (int i = 0; i < materials.Count; i++)
                if (isEmissionEnabled.Count > i)
                    materials[i].SetColor("_EmissionColor", Color.Lerp(!isEmissionEnabled[i] ? Color.black : emissionColors[i], flashColor, mod));

            for (int i = 0; i < materials.Count; i++)
                if (tints.Count > i)
                    materials[i].SetColor("_Color", Color.Lerp(tints[i], Color.black, mod));
        }
        else
        {
            for (int i = 0; i < materials.Count; i++)
                if (tints.Count > i)
                    materials[i].SetColor("_Color", Color.Lerp(tints[i], flashColor, mod));
        }

        if (Time.time > finishTime)
        {
            Destroy(this);
        }
    }
}
