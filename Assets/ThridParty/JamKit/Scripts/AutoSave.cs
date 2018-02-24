using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
	static public float		saveTimeout = 60 * 2; //2 mins

	static double			lastSave;

	static AutoSave()
	{
		EditorApplication.update += Update;
	}

	static void Update()
	{
		if (EditorApplication.timeSinceStartup - lastSave > saveTimeout)
			Save();
	}

	static void Save()
	{
		if (EditorApplication.isPlaying || EditorApplication.isPaused)
			return ;
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		AssetDatabase.SaveAssets();
		lastSave = EditorApplication.timeSinceStartup;
	}
}
