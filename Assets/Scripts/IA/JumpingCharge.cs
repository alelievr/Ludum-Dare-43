using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JumpingCharge : MonoBehaviour {

	public float chargejumppower = 10;
    public float prepjumptime = 0.1f;
	public float coefspeed = 2f;
	PlayerController ps;
	public float cd = 2;
	float	actualcd;
	float	TimeToCoolDown = 1f;
	Animator anim;
	Collider2D OuchZone;

	// Use this for initialization
	void Start () {
		ps = GetComponentInParent<PlayerController>();
		anim = GetComponentInParent<Animator>();
		OuchZone = GetComponentsInChildren<Collider2D>(true).Where(c => c.tag == "ouch").FirstOrDefault();
	}

	private void OnEnable()
	{
		isInJumpingCharging = false;
		actualcd = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (actualcd > 0)
			actualcd -= Time.deltaTime;
		if (ps.istapping && ps.TakingDamage == true)
		{
			StopCoroutine("JumpingCharging");
			anim.SetBool("istapping", false);
            anim.SetBool("ispreptapping", false);
            OuchZone.enabled = false;
			ps.istapping = false;
			ps.cannotmove = false;
			isInJumpingCharging = false;
			anim.SetBool("TakingTimeToCoolDown", false);
            actualcd = cd;
        }
    }

	bool isInJumpingCharging = false;

	IEnumerator JumpingCharging()
	{
		actualcd = cd;
		float move = (ps.facingLeft) ? -1 : 1;
		isInJumpingCharging = true;
		ps.cannotmove = true;
        anim.SetBool("ispreptapping", true);
        ps.Move(0);
        yield return new WaitForSeconds(prepjumptime);
        anim.SetBool("ispreptapping", false);

        ps.Move(move * 2);
        ps.istapping = true;
		ps.tryjump(chargejumppower);
		anim.SetBool("istapping", true);
		OuchZone.enabled = true;
        if (ps.TappingClip)
            ps.audiosource.PlayOneShot(ps.TappingClip, ps.tappingVolume);
		while (true)
		{
			if ((ps.grounded && ps.canJump) || ps.IsOuchstun)
				break ;
			ps.Move(move * coefspeed);
			yield return new WaitForEndOfFrame();
		}
		anim.SetBool("istapping", false);
		OuchZone.enabled = false;
		ps.istapping = false;
		ps.Move(0);
		anim.SetBool("TakingTimeToCoolDown", true);
        float timeTmp = 0;
        while (timeTmp < TimeToCoolDown)
        {
            yield return new WaitForEndOfFrame();
            ps.Move(0);
            timeTmp += Time.deltaTime;
        }
	
		anim.SetBool("TakingTimeToCoolDown", false);
		ps.cannotmove = false;
		actualcd = cd;
		isInJumpingCharging = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		OnTriggerStay2D(other);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (actualcd > 0 || isInJumpingCharging)
			return ;
		if (other.tag == "Player" && ps.grounded && ps.canJump && !ps.istapping)
			StartCoroutine(JumpingCharging());
	}
}
