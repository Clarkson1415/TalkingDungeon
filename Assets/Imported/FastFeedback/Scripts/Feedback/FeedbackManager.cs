using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    private static FeedbackManager _instance;

    public void DoThing()
    {

    }

    public static FeedbackManager Instance
    {
        get {
            if (_instance == null)
            {
                GameObject temp = new GameObject("FeedbackManager");
                _instance = temp.AddComponent<FeedbackManager>();
            }
            return _instance;
        }
    }



    public static void PlaySound(AudioClip clip)
    {
        Instance.StartCoroutine(Instance.PlaySoundCoroutine(clip));
    }

    public IEnumerator PlaySoundCoroutine(AudioClip clip)
    {
        AudioSource source = Instance.gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(source);
    }



    public static void SetAnimationTrigger(GameObject obj, string trigger)
    {
        obj.GetComponentInChildren<Animator>().SetTrigger(trigger);
    }

    public static void PlayAnimation(GameObject obj, string animation)
    {
        obj.GetComponentInChildren<Animator>().Play(animation);
    }



    public void PulseColor(GameObject obj, Color color)
    {

    }

    public IEnumerator PulseColorCoroutine (GameObject obj, Color color)
    {
        yield return null;
    }



    public static void PulseSize(GameObject obj, float increase)
    {
        Instance.StartCoroutine(Instance.PulseSizeCoroutine(obj, increase));
    }

    public IEnumerator PulseSizeCoroutine(GameObject obj, float increase)
    {
        
        Vector3 startScale = obj.transform.localScale;
        Vector3 endScale = obj.transform.localScale * increase;

        while (true)
        {
            float startTime = Time.time;
            float endTime = startTime + 0.6f / 2.0f;
            while (Time.time < endTime)
            {
                obj.transform.localScale = Vector3.Lerp(startScale, endScale, (Time.time - startTime) / (endTime - startTime));
                yield return null;
            }


            startTime = Time.time;
            endTime = Time.time + 0.6f / 2.0f;
            while (Time.time < endTime)
            {
                obj.transform.localScale = Vector3.Lerp(endScale, startScale, (Time.time - startTime) / (endTime - startTime));
                yield return null;
            }
            obj.transform.localScale = startScale;
            yield return null;
        }
        
    }



    public void EmitParticles(GameObject obj, ParticleSystem system)
    {
        //obj.GetComponentInChildren<ParticleSystem>().Emit()
    }



    public static void CreateObject(GameObject obj, Vector3 position, float timeUntilDestroy = 0)
    {
        GameObject myObj = Instantiate(obj, position, Quaternion.identity);
        if (timeUntilDestroy > 0)
        {
            Destroy(myObj, timeUntilDestroy);
        }
    }
}
