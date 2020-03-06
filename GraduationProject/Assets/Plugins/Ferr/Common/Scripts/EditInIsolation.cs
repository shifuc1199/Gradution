using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

// this class tracks Unity's play / stop play events, so we can enable and disable
[InitializeOnLoad]
class IsolationDeactivator {
#if UNITY_2017_2_OR_NEWER
	static IsolationDeactivator() {
		EditorApplication.playModeStateChanged += HandleChange;
	}
	static void HandleChange(PlayModeStateChange aChange) {
		if (aChange == PlayModeStateChange.ExitingEditMode) {
			EditInIsolation.DisableObjects();
		}
		if (aChange == PlayModeStateChange.EnteredEditMode) {
			EditInIsolation.EnableObjects();
		}
	}
#else
	static bool wasPlayingPre  = false;
	static bool wasPlayingPost = false;

	static IsolationDeactivator() {
		EditorApplication.playmodeStateChanged = HandleChange;
	}

	static void HandleChange() {
		// Disable objects -before- we start playing. Don't want Awake events.
		if (!wasPlayingPre && EditorApplication.isPlayingOrWillChangePlaymode) {
			EditInIsolation.DisableObjects();
		}
		// Enable objects -after- playing has ended. Don't want Unity to restore original disabled state afterwards
		if (wasPlayingPost && !EditorApplication.isPlaying) {
			EditInIsolation.EnableObjects();
		}
		wasPlayingPre  = EditorApplication.isPlayingOrWillChangePlaymode;
		wasPlayingPost = EditorApplication.isPlaying;
	}
#endif
}
#endif

public class EditInIsolation : MonoBehaviour {
	[HideInInspector]
	[SerializeField] bool[] _wasEnabled;

	#if UNITY_EDITOR
	[MenuItem("Assets/Edit In Isolation", false, 30)]
	static void Edit() {
		Object[]     objs           = Selection.gameObjects;
		List<Object> result         = new List<Object>();
		Scene        isolationScene = GetIsolationScene();

		for (int i = 0; i < objs.Length; i++) {
			// only game objects
			if (!(objs[i] is GameObject))
				continue;

			// make sure this object is a prefab, not a scene object
			PrefabType t = PrefabUtility.GetPrefabType(objs[i]);
			if (t == PrefabType.None || t == PrefabType.DisconnectedPrefabInstance || t == PrefabType.MissingPrefabInstance)
				continue;

			// add it to the scene as a prefab, and track it for pinging
			GameObject go = PrefabUtility.InstantiatePrefab(objs[i], isolationScene) as GameObject;
			result.Add(go);
		}

		// if we made anything, ping it, and select 'em
		if (result.Count > 0) {
			EditorGUIUtility.PingObject(result[result.Count-1]);
			Selection.objects = result.ToArray();
		} 
	}
	static EditInIsolation HasIsolationScene() {
		EditInIsolation edit = null;

		// find the isolation scene
		for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
			Scene scene = EditorSceneManager.GetSceneAt(i);
			if (!scene.isLoaded)
				continue;

			GameObject[] roots = scene.GetRootGameObjects();
			edit = GetFrom(roots);
			if (edit != null)
				break;
		}
		return edit;
	}
	static Scene GetIsolationScene() {
		EditInIsolation edit = null;
		Scene           result = default(Scene);

		// find the isolation scene
		for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
			result = EditorSceneManager.GetSceneAt(i);
			if (!result.isLoaded)
				continue;

			GameObject[] roots = result.GetRootGameObjects();
			edit = GetFrom(roots);
			if (edit != null) 
				break;
		}

		// no isolation scene found, go ahead and make one
		if (edit == null) {
			Scene oldScene = EditorSceneManager.GetActiveScene();
			Scene scene    = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
			EditorSceneManager.SetActiveScene(scene);

			GameObject go = new GameObject("_EditInIsolationScene");
			go.AddComponent<EditInIsolation>();
			EditorSceneManager.SetActiveScene(oldScene);

			result = scene;
		}
		return result;
	}
	public static void DisableObjects() {
		EditInIsolation edit = HasIsolationScene();
		if (edit == null)
			return;

		GameObject[] objs = GetIsolationScene().GetRootGameObjects();
		edit._wasEnabled = new bool[objs.Length];

		// disable all objects, but track their previous states in an object that will serialize/maintain the data
		for (int i = 0; i < objs.Length; i++) {
			edit._wasEnabled[i] = objs[i].activeSelf;
			objs[i].SetActive(false);
		}
	}
	public static void EnableObjects() {
		EditInIsolation edit = HasIsolationScene();
		if (edit == null)
			return;

		GameObject[] objs = GetIsolationScene().GetRootGameObjects();

		// restore previous state
		for (int i = 0; i < objs.Length; i++) {
			objs[i].SetActive(edit._wasEnabled[i]);
		}
	}
	static EditInIsolation GetFrom(GameObject[] objs) {
		EditInIsolation result = null;
		for (int i = 0; i < objs.Length; i++) {
			result = objs[i].GetComponent<EditInIsolation>();
			if (result != null)
				break;
		}
		return result;
	}
	#endif
}