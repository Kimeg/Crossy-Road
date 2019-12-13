using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject scdummy;
    private Scrollbar sc;
    private bool muted = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        scdummy = GameObject.Find("Canvas").transform.Find("OptionUI").gameObject.transform.Find("SoundScrollbar").gameObject;
        sc = scdummy.GetComponent<Scrollbar>();
    }

    private void Update()
    {
        if (muted)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = sc.value;
        }
        //Debug.Log("스크롤 값 : " + sc.value.ToString() + " - - - " + "실제 볼륨 : " + audioSource.volume.ToString());
    }

    public void MuteBTN()
    {
        if (muted)
        {
            muted = false;
        }
        else
        {
            muted = true;
        }
    }
}