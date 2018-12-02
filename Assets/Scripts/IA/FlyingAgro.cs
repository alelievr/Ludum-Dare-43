using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAgro : FlyingBaseAgro
{
	public float ydecal = 1;

	// Use this for initialization
	// void Start () {
		
	// }
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void FixedUpdate ()
	{
		move = 0;
		movey = 0;
		if (!base.cannotmove)
		{
            if (Cible && !istapping)
			{
				float distance = Vector2.Distance(Cible.position, transform.position);
				Vector2 realCible = (Vector2)Cible.position + ((new Vector2((Cible.position.x - transform.position.x < 0) ? 1 : -1, ydecal)).normalized * perfectdistancetocible);
                if (distance > MaxDistance)
                {
                    Cible = null; // peut etre active reactive qaund respawn pres
                    return ;
                }
				Vector2 dir = (realCible - (Vector2)transform.position).normalized;
				move = dir.x;
				movey = dir.y;
			}
		}
		PCFixedUpdate();
	}
}
