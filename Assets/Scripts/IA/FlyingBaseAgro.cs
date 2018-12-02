using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBaseAgro : Agro
{
    override protected void GroundCheck()
	{
        grounded = true;
		IsOnLadder = true;
	}
}
