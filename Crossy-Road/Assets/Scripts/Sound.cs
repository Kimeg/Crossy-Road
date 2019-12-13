using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioSource audioFile;
    public AudioClip[] sounds;

    // Start is called before the first frame update
    void Start()
    {
        audioFile = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SoundPlay(int num)
    {
        audioFile.PlayOneShot(sounds[num]);
    }

    public void StopPlay()
    {
        audioFile.Stop();
    }
}
