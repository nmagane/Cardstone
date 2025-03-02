using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;

    public void PlayMatchNotification()
    {
        source.PlayOneShot(clips[0]);
    }

}
