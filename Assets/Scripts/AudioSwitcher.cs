using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitcher : MonoBehaviour
{
    public AudioSource  intro;
    public AudioSource  loop;
    
    void Start()
    {
        loop.PlayScheduled(intro.time);
    }

    void Update()
    {
    }
}
