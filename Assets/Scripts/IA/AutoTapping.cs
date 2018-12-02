using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoTapping : MonoBehaviour {

	// Use this for initialization

    Animator anim;
    public float timeofanimbeforetap = 0.25f;
    public float timeafter = 0.25f;
    public float SimulateIn = -1;

    Agro pc;
    Rigidbody2D rb;
    Collider2D col;
    Collider2D colCible = null;
    Vector2 actualSpeed = Vector2.zero;
    Vector2 lastPos;

    Vector3 defaultPos;
    private void Start()
    {
        anim = GetComponentInParent<Animator>();
		pc = GetComponentInParent<Agro>();
        if (SimulateIn > 0)
        {
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            lastPos = transform.position;
            defaultPos = transform.localPosition;

        }
    }
    private void FixedUpdate()
    {   
        if (SimulateIn > 0)
        {
            actualSpeed = ((Vector2)transform.parent.position - lastPos) / Time.fixedDeltaTime * -Mathf.Sign(transform.lossyScale.x);
            if (pc.Cible)
            {
                if (colCible == null)
                    colCible = pc.Cible.GetComponents<Collider2D>().Where(c => !c.isTrigger).FirstOrDefault();
                Vector3 pos = transform.GetChild(0).position;
                col.transform.localPosition = defaultPos + ((Vector3)actualSpeed * SimulateIn);
                transform.GetChild(0).position = pos;
            }
            lastPos = transform.parent.position;
        }

    }

	IEnumerator Tapping()
    {
        pc.istapping = true;
        anim.SetBool("istapping", true);

        // move = transform.position.x - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)).x; // tape ducoter de la sourie (en gros la ca sert a rien)
        // if (!istapping && move > 0 && !facingRight)
        //     Flip();
        // else if (!istapping && move < 0 && facingRight)
        //     Flip();
        for (float i = 0; i < 0.25f; i += Time.deltaTime)
        {
            if (pc.istapping == false)
                StopTapping();
            yield return new WaitForEndOfFrame();
        }
        if (pc.TappingClip && pc.istapping)
            pc.audiosource.PlayOneShot(pc.TappingClip, pc.tappingVolume);
        for (float i = 0; i < 0.25f; i += Time.deltaTime)
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
        StopCoroutine(Tapping());
        pc.istapping = false;
    }

	
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Player" && pc.istapping == false && pc.IsOuchstun == false)
        {
			StartCoroutine(Tapping());
        }
	}
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        colCible = null;
    }
}
