using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyingMountedAgro : FlyingBaseAgro
{
    // Start is called before the first frame update
    public float timeOfCharge = 1.5f;
    public float MultVitesseForCharge = 2;
    public float timePrepCharge = 0.2f;
    public float DistanceToBeginCharge = 10;

    bool isInCharge = false;
    Vector2 dir = Vector2.zero;

    Vector2 chargeDir;
    IEnumerator Charge()
    {
        isInCharge = true;
        float baseSpeed = maxSpeed;
        maxSpeed = 0;
        yield return new WaitForSeconds(timePrepCharge);
        maxSpeed = baseSpeed * MultVitesseForCharge;
        yield return new WaitForSeconds(timeOfCharge);
        maxSpeed = baseSpeed;
        isInCharge = false;
    }

	protected override void FixedUpdate ()
	{
		move = 0;
		movey = 0;
		if (!base.cannotmove)
		{
            if (Cible)
			{
                float distance = Vector2.Distance(Cible.position, transform.position);
                if (distance > MaxDistance)
                {
                    Cible = null; // peut etre active reactive qaund respawn pres
                    return ;
                }
                if (!isInCharge)
                {
                    if (distance > DistanceToBeginCharge)
                        dir = ((Vector2)Cible.position - (Vector2)transform.position).normalized;
                    else
                        StartCoroutine(Charge());
                }
                move  = dir.x;
                movey = dir.y;
			}
		}
		PCFixedUpdate();
	}
}
