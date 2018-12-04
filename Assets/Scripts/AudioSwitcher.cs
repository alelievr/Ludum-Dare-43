using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitcher : MonoBehaviour
{
    public AudioSource  intro;
    public AudioSource  loop;
    
    void Start()
    {
        Debug.Log("intro.time: " + intro.clip.length);
        Debug.Log("dspTime; " + AudioSettings.dspTime);
        loop.PlayScheduled(AudioSettings.dspTime + intro.clip.length);
    }

    void Update()
    {
    }
}
