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

    private void Start()
    {
        TMP = this.GetComponent<TMP_Text>();

        TMP.ForceMeshUpdate(); // Ensure text mesh is ready
        TMP_TextInfo textInfo = TMP.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 0; // Make transparent
            }
        }

        TMP.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void PlaySavedAnimation()
    {
        this.TMP.text = "saved game";
        StartCoroutine(FadeInLetters());
    }

    IEnumerator FadeInLetters()
    {
        this.TMP.ForceMeshUpdate();
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
        yield return new WaitForSeconds(1f); // wait 1 second before fading out

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
}
