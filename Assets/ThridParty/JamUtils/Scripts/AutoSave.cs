using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
	static public float		saveTimeout = 60 * 2; //2 mins

	static float			lastSave;

	static AutoSave()
	{
		EditorApplication.update += Update;
	}

	static void Update()
	{
		if (Time.time - lastSave > saveTimeout)
			Save();
	}

	static void Save()
	{
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		AssetDatabase.SaveAssets();
		lastSave = Time.time;
	}
}
