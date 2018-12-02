using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitcher : MonoBehaviour
{
    public AudioSource  intro;
    public AudioSource  loop;
    
    bool                isLooping = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (!intro.isPlaying && !isLooping)
        {
            loop.Play();
            isLooping = true;
        }
    }
}
