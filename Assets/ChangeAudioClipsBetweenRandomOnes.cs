using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAudioClipsBetweenRandomOnes : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips = new();
    [SerializeField] private float startDelay = 0.2f;
    [SerializeField] private float timeBetween = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(PlayRandomAudio());
    }

    IEnumerator PlayRandomAudio()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            int randomIndex = Random.Range(0, audioClips.Count);
            audioSource.clip = audioClips[randomIndex];
            //audioSource.Play(); No dont play audio in this script. See Blacksmith the animation plays it for now to stay in sync.
            yield return new WaitForSeconds(timeBetween);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
