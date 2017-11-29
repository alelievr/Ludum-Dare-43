using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
	public bool		fadeSoud = false;
	public float	fadeTime = .5f;

	public bool		interuptLoading = false;

	public string	objectsToFadeTag = "FadeTranstion";

	bool			loading;
	string			toLoad;

	public void LoadScene(string name)
	{
		if (loading && !interuptLoading)
			return ;
		
		if (interuptLoading)
			StopCoroutine("LoadSceneCoroutine");
		
		toLoad = name;
		
		StartCoroutine("LoadSceneCoroutine");
	}

	void StartFadeIn()
	{
		GameObject[] toFadeIn = GameObject.FindGameObjectsWithTag(objectsToFadeTag);

		foreach (var obj in toFadeIn)
		{
			Image img = obj.GetComponent< Image >();
			if (img != null)
			{
				Color end = img.color;
				end.a = 1;
				Color start = img.color;
				start.a = 0;
				img.color = start;
				img.DOColor(end, fadeTime);
			}
			if (fadeSoud)
			{
				AudioSource audio = obj.GetComponent< AudioSource >();
				if (audio != null)
					audio.DOFade(0, fadeTime);
			}
		}
	}

	void StartFadeOut()
	{
		GameObject[] toFadeIn = GameObject.FindGameObjectsWithTag(objectsToFadeTag);

		foreach (var obj in toFadeIn)
		{
			Image img = obj.GetComponent< Image >();
			if (img != null)
			{
				Color end = img.color;
				end.a = 0;
				Color start = img.color;
				start.a = 1;
				img.color = start;
				img.DOColor(end, fadeTime);
			}
			if (fadeSoud)
			{
				AudioSource audio = obj.GetComponent< AudioSource >();
				if (audio != null)
					audio.DOFade(1, fadeTime);
			}
		}
	}

	IEnumerator LoadSceneCoroutine()
	{
		loading = true;

		StartFadeIn();

		yield return new WaitForSeconds(fadeTime);

		SceneManager.LoadScene(toLoad);

		//wait for the scene to load
		yield return new WaitForSeconds(0.0f);

		StartFadeOut();

		yield return new WaitForSeconds(fadeTime);

		loading = false;
	}

}
