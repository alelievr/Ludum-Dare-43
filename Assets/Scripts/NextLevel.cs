using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : SceneTransition {

	public string nexTSceneName;
	public string tagAccepted = "Player";
	// Use this for initialization


	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == tagAccepted)
		{
			base.LoadScene(nexTSceneName);
		}

	}
}
