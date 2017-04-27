using UnityEngine;
using UnityEditor;

public class ReplacePrefab : EditorWindow
{
	GameObject prefab;
	GameObject go;

	[MenuItem ("Prefabs/ReplacePrefab")]
	static void OpenWindow ()
	{
		EditorWindow.GetWindow (typeof(ReplacePrefab));
	}

	void OnGUI ()
	{
		go = EditorGUILayout.ObjectField ("GameObject", go, typeof(GameObject), true) as GameObject;
		prefab = EditorGUILayout.ObjectField ("Target Prefab", prefab, typeof(GameObject), false) as GameObject;
		if (GUI.changed && prefab != null) {
			PrefabType prefabType = PrefabUtility.GetPrefabType (prefab);
			if (prefabType != PrefabType.Prefab)
				prefab = null;
		}
		if (go != null && prefab != null) {
			if (GUILayout.Button ("Replace")) {
				PrefabUtility.ReplacePrefab (go, prefab, ReplacePrefabOptions.ConnectToPrefab | ReplacePrefabOptions.ReplaceNameBased);
				GUIUtility.ExitGUI ();
			}
		}
	}


}