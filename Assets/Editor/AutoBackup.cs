using UnityEngine;
using UnityEditor;
public class AutoBackup : EditorWindow
{
	static float saveTime = 30;
	static float nextSave = 0f;

	// Add menu named "My Window" to the Window menu
	

	[InitializeOnLoad]
	public class Startup
	{
		static Startup()
		{
			EditorApplication.update += Update;
		}
	}

	[MenuItem("Tools/AutoBackup")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		AutoBackup window = (AutoBackup)EditorWindow.GetWindow(typeof(AutoBackup));
		window.Show();
	}

	static void Update()
	{
		if (EditorApplication.timeSinceStartup > nextSave)
		{
			string[] path = EditorApplication.currentScene.Split(char.Parse("/"));

			path[path.Length - 1] = "AutoSave_" + path[path.Length - 1];

			EditorApplication.SaveScene(string.Join("/", path), true);
			Debug.Log("Saved Scene");
			nextSave = (float)EditorApplication.timeSinceStartup + saveTime;
		}
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);

		EditorGUILayout.LabelField("Save Each:", saveTime + "seconds");
		int timeToSave = (int)nextSave - (int)EditorApplication.timeSinceStartup;

		EditorGUILayout.LabelField("Next Save:", timeToSave.ToString() + " seconds");

		this.Repaint();
	}
}