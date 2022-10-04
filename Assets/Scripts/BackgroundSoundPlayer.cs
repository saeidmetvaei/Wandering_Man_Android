using UnityEngine;
using System.Collections;

public class BackgroundSoundPlayer : MonoBehaviour
{


    public AudioSource Audiosource;
    public AudioClip Song;

    void Start()
    {

        Audiosource.clip = Song;
        Audiosource.Play();

    }

}
