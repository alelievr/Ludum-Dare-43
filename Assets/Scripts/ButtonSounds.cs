using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip    hoverClip;
    public AudioClip    pressedClip;

    AudioSource     source;
    Button       button;

    void Start()
    {
        source = GetComponent< AudioSource >();
        button = GetComponent< Button >();
        button.onClick.AddListener(ClickCallback);
    }

    void ClickCallback()
    {
        source.PlayOneShot(pressedClip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        source.PlayOneShot(hoverClip);
    }
}
