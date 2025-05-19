using System.Collections;
using TMPro;
using UnityEngine;

public class SavedAnimationText : MonoBehaviour
{
    private TMP_Text TMP;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float delayBetweenLetters = 0.5f;

    private void Awake()
    {
        TMP = this.GetComponent<TMP_Text>();
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

        this.gameObject.SetActive(false);
    }
}
