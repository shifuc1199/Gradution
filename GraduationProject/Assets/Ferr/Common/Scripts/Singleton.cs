using UnityEngine;
using System.Collections;

namespace Ferr {
	public abstract class Singleton<T> : MonoBehaviour where T:Component{
		private static T _instance;
		public  static T  Instance {
			get {
				if (_instance == null)
					EnsureInstantiated();

				return _instance;
			}
		}
		public static bool Instantiated {
			get { return _instance != null; }
		}
		public static void EnsureInstantiated() {
			if (_instance == null) {
				// singletons can get disconnected when unity compile events occur, this will serve as a re-connection if that occurs
				_instance = GameObject.FindObjectOfType<T>() as T;
				// if it's still null, see if we can instantiate it from the resources folder!
				if (_instance == null) {
					GameObject obj = Resources.Load("Singletons/" + typeof(T).Name) as GameObject;
					if (obj != null) {
						GameObject go = GameObject.Instantiate(obj);
						_instance = go.GetComponent<T>();
					}
				}
				// and if there was no prefab, lets just try a plain create and go!
				if (_instance == null) {
					GameObject go = new GameObject("_" + typeof(T).Name);
					_instance = go.AddComponent(typeof(T)) as T;
				}
				// and if all else fails, start spouting smoke
				if (_instance == null) {
					Debug.LogErrorFormat("Couldn't connect or create singleton {0}!", typeof(T).Name);
				}
			}
		}

		protected virtual void Awake() {
			if (_instance != null && _instance != this) {
				Debug.LogErrorFormat(gameObject, "Creating a new instance of a singleton [{0}] when one already exists!", typeof(T).Name);
				gameObject.SetActive(false);
				return;
			}
			_instance = GetComponent<T>();
		}
		protected virtual void OnDestroy() {
			_instance = null;
		}
	}
}