using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadUnique : MonoBehaviour
{
	[HideInInspector]
	public bool	destroyOnLoad = false;

	void Start()
	{
		var objs = FindObjectsOfType< DontDestroyOnLoadUnique >();

		foreach (var obj in objs)
			if (obj != this)
				obj.destroyOnLoad = true;

		DontDestroyOnLoad(gameObject);

		SceneManager.sceneLoaded += OnLoadCallback;
	}

	void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnLoadCallback;
	}

	void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
	{
		if (destroyOnLoad)
			Destroy(gameObject);
	}
}
