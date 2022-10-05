using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }

    public AudioSource audioSourceSE;
    public AudioClip buttonSe;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void playButtonSe()
    {
        audioSourceSE.PlayOneShot(buttonSe);
    }
}
