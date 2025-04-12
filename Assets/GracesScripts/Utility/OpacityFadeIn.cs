using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpacityFadeIn : MonoBehaviour
{
    private Image image;
    private TMP_Text text;
    private float timeDelay = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        image = this.GetComponent<Image>();
        text = this.GetComponent<TMP_Text>();

        if (image == null && text == null)
        {
            Debug.Log("opacity fade in tried to start but text and image are null");
            return;
        }

        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            StartCoroutine(OpacityFadeInImage());
        }
        else if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            StartCoroutine(OpacityFadeInText());
        }
        else
        {
            throw new ArgumentOutOfRangeException($"opacity fade in only supports image and text using on {this.gameObject.name}");
        }
    }

    private IEnumerator OpacityFadeInText()
    {
        while (text.color.a < 1)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + 0.05f);
            yield return new WaitForSeconds(timeDelay);
        }
    }

    private IEnumerator OpacityFadeInImage()
    {
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + 0.05f);
            yield return new WaitForSeconds(timeDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
