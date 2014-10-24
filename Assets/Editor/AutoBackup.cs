using UnityEngine;
using UnityEditor;
using System.IO;
public class AutoBackup : EditorWindow
{
	static bool enable = false;
	static float saveTime;
	static float nextSave = 0f;

	static string autoSavePrefix = "AutoSave_";
	static string pathToSaveScenesTo = "Assets/Scenes/AutoBackup/";

	static int numberOfSaves = 3;
	static int currentSaveNumber = 0;

	static bool saveAssets = true;
	static bool saveOnPlay = true;

	[InitializeOnLoad]
	public class Startup
	{
		static Startup()
		{

			EditorApplication.update += Update;
			EditorApplication.playmodeStateChanged += PlaymodeStateChanged;

			enable = EditorPrefs.GetBool("enable", false);
			saveTime = EditorPrefs.GetFloat("saveTime", 300);
			autoSavePrefix = EditorPrefs.GetString("prefix", "AutoSave_");
			pathToSaveScenesTo = EditorPrefs.GetString("path", "Assets/Scenes/AutoBackup/");
			numberOfSaves = EditorPrefs.GetInt("numberOfSaves", 3);
			saveAssets = EditorPrefs.GetBool("saveAssets", true);
			saveOnPlay = EditorPrefs.GetBool("saveOnPlay", true);
		}
	}

	[MenuItem("Tools/AutoBackup Settings")]
	static void Init()
	{
		AutoBackup window = (AutoBackup)EditorWindow.GetWindow(typeof(AutoBackup));
		window.Show();
	}

	static void Update()
	{
		if (enable)
		{
			if (EditorApplication.timeSinceStartup > nextSave)
			{
				Save();
				nextSave = (float)EditorApplication.timeSinceStartup + saveTime;
			}
		}
	}

	static void PlaymodeStateChanged()
	{
		if (enable && saveOnPlay)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				Save();
			}
		}
	}

	static void Save()
	{
		string[] path = EditorApplication.currentScene.Split(char.Parse("/"));

		if (currentSaveNumber < numberOfSaves)
		{
			currentSaveNumber += 1;
		}
		else if (currentSaveNumber == numberOfSaves)
		{
			currentSaveNumber = 1;
		}

		string incrementedPrefix = autoSavePrefix + currentSaveNumber.ToString() + "_";
		string filename = incrementedPrefix + path[path.Length - 1];

		Debug.Log(path[path.Length - 1]);

		if (Directory.Exists(pathToSaveScenesTo) && filename == incrementedPrefix)
		{
			EditorApplication.SaveScene();
			currentSaveNumber = 0;
		}
		else if (!Directory.Exists(pathToSaveScenesTo) && filename == incrementedPrefix)
		{
			Directory.CreateDirectory(pathToSaveScenesTo);
			EditorApplication.SaveScene(pathToSaveScenesTo + filename, true);
			EditorApplication.SaveScene();

		}
		else
		{
			EditorApplication.SaveScene(pathToSaveScenesTo + filename, true);
			EditorApplication.SaveScene();
		}

		if (saveAssets)
		{
			EditorApplication.SaveAssets();
		}

		Debug.Log("Saved Scene");

	}

	void OnGUI()
	{
		GUILayout.Label("AutoBackup Settings", EditorStyles.boldLabel);

		GUILayout.Label("");
		GUILayout.Label("Enable AutoBackup?", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Enable");
		enable = EditorGUILayout.ToggleLeft("", enable);
		GUILayout.EndHorizontal();

		GUILayout.Label("");
		GUILayout.Label("Timing", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		saveTime = EditorGUILayout.FloatField("Save every:", saveTime);
		GUILayout.Label("seconds");
		GUILayout.EndHorizontal();

		int timeToSave;
		if (enable)
		{
			timeToSave = (int)nextSave - (int)EditorApplication.timeSinceStartup;
		}
		else
		{
			timeToSave = (int)saveTime;
		}

		EditorGUILayout.LabelField("Next Save:", timeToSave.ToString() + " seconds");


		GUILayout.Label("");
		GUILayout.Label("Save Path", EditorStyles.boldLabel);

		autoSavePrefix = EditorGUILayout.TextField("AutoBackup Prefix", autoSavePrefix);
		pathToSaveScenesTo = EditorGUILayout.TextField("Path:", pathToSaveScenesTo);

		GUILayout.Label("");
		GUILayout.Label("Incremental Saves", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Number of backups");
		numberOfSaves = EditorGUILayout.IntSlider(numberOfSaves, 1, 20);
		GUILayout.EndHorizontal();
		EditorGUILayout.LabelField("Save Number:" + currentSaveNumber + " of " + numberOfSaves);

		GUILayout.Label("");
		GUILayout.Label("Options", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save Assets?");
		saveAssets = EditorGUILayout.ToggleLeft("", saveAssets);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save Before Play?");
		saveOnPlay = EditorGUILayout.ToggleLeft("", saveOnPlay);
		GUILayout.EndHorizontal();

		GUILayout.Label("");

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Save Settings"))
		{
			SaveAutoBackupSettings();
		}
		if (GUILayout.Button("Restore Defaults"))
		{
			RestoreDefaultSettings();
		}
		GUILayout.EndHorizontal();
		this.Repaint();
	}

	public void SaveAutoBackupSettings()
	{
		EditorPrefs.SetBool("enable", enable);
		EditorPrefs.SetFloat("saveTime", saveTime);
		EditorPrefs.SetString("prefix", autoSavePrefix);
		EditorPrefs.SetString("path", pathToSaveScenesTo);
		EditorPrefs.SetInt("numberOfSaves", numberOfSaves);
		EditorPrefs.SetBool("saveAssets", saveAssets);
		EditorPrefs.SetBool("saveOnPlay", saveOnPlay);
	}

	public void RestoreDefaultSettings()
	{
		enable = false;
		saveTime = 300f;
		nextSave = saveTime;

		autoSavePrefix = "AutoSave_";
		pathToSaveScenesTo = "Assets/Scenes/AutoBackup/";

		numberOfSaves = 3;
		currentSaveNumber = 0;

		saveAssets = true;
		saveOnPlay = true;
	}
}