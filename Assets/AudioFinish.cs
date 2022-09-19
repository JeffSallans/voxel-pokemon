using UnityEngine;
using System.Collections;

// https://gamedev.stackexchange.com/a/134003
[RequireComponent(typeof(AudioSource))]
public class AudioFinish : MonoBehaviour
{
    public AudioClip startClip;
    // TODO: hack that makes this less flexible so I don't have to learn UnityEvents
    public AudioClip loopClip;
    private AudioSource audioSource;
    //public UnityEvent onFinishSound;

    private void Awake()
    {
        Initialize();
        StartCoroutine(WaitForSound());
    }

    public void Initialize()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = startClip;
        audioSource.Play();
    }

    IEnumerator WaitForSound()
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        print($"{startClip.name} finished");
        //onFinishSound.Invoke();
        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}