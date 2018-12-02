using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public float durationOfAnim = 1;
    public float cd = 1;
    public GameObject Spell;
    public Vector3 offset = new Vector3(0,0,0);
    public Transform OverideofCible = null;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1;
    Animator anim;
    
    Agro pc;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
		pc = GetComponentInParent<Agro>();
    }
	IEnumerator magic()
    {
        pc.istapping = true;
        anim.SetBool("istapping", true);

        for (float i = 0; i < durationOfAnim; i += Time.deltaTime)
        {
            if (pc.istapping == false)
                StopTapping();
            yield return new WaitForEndOfFrame();
        }
        if (clip && pc.istapping)
            pc.audiosource.PlayOneShot(clip, volume);
        GameObject.Instantiate(Spell, ((OverideofCible) ? OverideofCible.position : pc.Cible.position) + offset, Spell.transform.rotation);
        for (float i = 0; i < cd; i += Time.deltaTime)
        {
            if (pc.istapping == false)
                StopTapping();
            yield return new WaitForEndOfFrame();
        }
        StopTapping();
    }

    void StopTapping()
    {
        anim.SetBool("istapping", false);
        StopCoroutine(magic());
        pc.istapping = false;
    }
    void Update()
    {
        if (pc.Cible && !pc.istapping)
            StartCoroutine(magic());
    }
}
