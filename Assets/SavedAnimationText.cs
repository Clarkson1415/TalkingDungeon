using System.Collections;
using System.Globalization;
using System.Xml;
using TMPro;
using UnityEngine;

public class SavedAnimationText : MonoBehaviour
{
    private TMP_Text TMP;
    [SerializeField] float fadeDuration = 0.3f;
    [SerializeField] float delayBetweenLetters = 0.1f;

    private void Awake()
    {
        TMP = GetComponent<TMP_Text>();
        this.MakeTransparent();
    }

    private void Start()
    {
        TMP = GetComponent<TMP_Text>();
        MakeTransparent();
    }


    [SerializeField] private bool FadeInIndivualLetters = false;
    [SerializeField] float FadeTogetherDuration = 1f;
    [SerializeField] float WaitBeforeFadeOut = 1f;

    public void PlaySavedAnimation()
    {
        this.TMP.text = "saved game";

        if (FadeInIndivualLetters)
        {
            StartCoroutine(FadeInLetters());
        }
        else
        {
            StartCoroutine(FadeInAllLetters());
        }
    }

    public void MakeTransparent()
    {
        TMP = GetComponent<TMP_Text>();
        TMP_TextInfo textInfo = this.TMP.textInfo;
        // Make all characters transparent initially
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 0;
            }
        }
    }

    IEnumerator FadeInLetters()
    {
        this.TMP.ForceMeshUpdate();
        TMP_TextInfo textInfo = this.TMP.textInfo;

        // Make all characters transparent initially
        MakeTransparent();

        this.TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Fade in each letter one by one
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Clamp01(elapsed / fadeDuration);
                byte byteAlpha = (byte)(alpha * 255);

                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j].a = byteAlpha;
                }

                this.TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure it's fully opaque at the end
            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 255;
            }
            this.TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return new WaitForSeconds(delayBetweenLetters);
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // Optional: Wait before fading out
        yield return new WaitForSeconds(WaitBeforeFadeOut);

        // Fade out all visible characters
        float fadeOutDuration = 0.5f;
        float elapsed = 0f;

        this.TMP.ForceMeshUpdate(); // Update in case text changed
        var textInfo = this.TMP.textInfo;

        while (elapsed < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            byte byteAlpha = (byte)(alpha * 255);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int meshIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j].a = byteAlpha;
                }
            }

            TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeInAllLetters()
    {
        var duration = this.FadeTogetherDuration;

        TMP.ForceMeshUpdate(); // Ensure mesh is current
        TMP_TextInfo textInfo = TMP.textInfo;

        float elapsed = 0f;

        // Initialize all alphas to 0
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
                vertexColors[vertexIndex + j].a = 0;
        }

        TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Animate alpha from 0 to 255
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            byte byteAlpha = (byte)(alpha * 255);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int meshIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

                for (int j = 0; j < 4; j++)
                    vertexColors[vertexIndex + j].a = byteAlpha;
            }

            TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final alpha is fully opaque
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
                vertexColors[vertexIndex + j].a = 255;
        }

        TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        StartCoroutine(FadeOut());
    }

}
