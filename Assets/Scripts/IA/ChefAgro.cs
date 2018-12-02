using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChefAgro : MonoBehaviour {

	bool done = false;
	public List<Agro> Subordonate;



	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player")
		{
			Subordonate.ForEach(s => s.Cible = other.transform);
			done = true;
		}
	}
}
