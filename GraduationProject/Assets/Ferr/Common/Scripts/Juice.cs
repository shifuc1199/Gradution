using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Ferr {
	public enum JuiceType {
		ScaleX = 1,
		ScaleY = 2,
		ScaleZ = 4,
		ScaleXYZ = 7,
	
		TranslateX = 8,
		TranslateY = 16,
		TranslateZ = 32,
		TranslateXYZ = 56,
	
		RotationX = 64,
		RotationY = 128,
		RotationZ = 256,
		RotationXYZ = 448
	}
	
	[Serializable]
	public class JuiceData {
		public JuiceType type;
		public Transform transform;
		public float     start;
		public float     end;
		public float     duration;
		public float     startTime;
		public bool      relative;
		public AnimationCurve curve;
		public Action         callback;
	
		public bool Update() {
			if (transform == null) return true;
			float percent = Mathf.Min(( Time.time - startTime ) / duration, 1f);
			float value   = start + curve.Evaluate(percent) * (end - start);
			if (relative) {
				float prevPercent = Mathf.Max (0, Mathf.Min( ((Time.time-Time.deltaTime) - startTime) / duration, 1f ));
				value             = value - (start + curve.Evaluate(prevPercent) * (end - start));
			}
			
			JuiceType t = type;
			bool    doPos   = false;
			bool    doScale = false;
			bool    doRot   = false;
			Vector3 pos     = relative ? Vector3.zero : transform.localPosition;
			Vector3 scale   = relative ? Vector3.zero : transform.localScale;
			Vector3 rot     = relative ? Vector3.zero : transform.eulerAngles;
			
			// translation
			if ((t & JuiceType.TranslateX) > 0) {
				pos.x = value;
				doPos = true;
			}
			if ((t & JuiceType.TranslateY) > 0) {
				pos.y = value;
				doPos = true;
			}
			if ((t & JuiceType.TranslateZ) > 0) {
				pos.z = value;
				doPos = true;
			}
			
			// scale
			if ((t & JuiceType.ScaleX) > 0) {
				scale.x = value;
				doScale = true;
			}
			if ((t & JuiceType.ScaleY) > 0) {
				scale.y = value;
				doScale = true;
			}
			if ((t & JuiceType.ScaleZ) > 0) {
				scale.z = value;
				doScale = true;
			}
			
			// rotation
			if ((t & JuiceType.RotationX) > 0) {
				rot.x = value;
				doRot = true;
			}
			if ((t & JuiceType.RotationY) > 0) {
				rot.y = value;
				doRot = true;
			}
			if ((t & JuiceType.RotationZ) > 0) {
				rot.z = value;
				doRot = true;
			}
			
			if      (doPos   && relative) transform.localPosition += pos;
	        else if (doPos              ) transform.localPosition  = pos;
			
			if      (doScale && relative) transform.localScale  += scale;
			else if (doScale            ) transform.localScale   = scale;
			
			if      (doRot   && relative) transform.localEulerAngles += rot;
			else if (doRot              ) transform.localEulerAngles  = rot;
	
			return percent >= 1.0f;
		}
		
		public void Cancel() {
			startTime = -10000;
			Update();
		}
	}
	
	[Serializable]
	public class JuiceDataColor {
		public Material renderer;
		public Color    start;
		public Color    end;
		public float    duration;
		public float    startTime;
		public AnimationCurve curve;
		public Action         callback;
	
		public bool Update() {
			if (renderer == null) return true;
			
			float percent = Mathf.Min(( Time.time - startTime ) / duration, 1f);
			Color val     = Color.Lerp(start, end, curve.Evaluate(percent));
			
			renderer.color = val;
	
			return percent >= 1.0f;
		}
		
		public void Cancel() {
			startTime = -10000;
			Update();
		}
	}
	
	public class Juice : MonoBehaviour {
		#region Singleton
		private static Juice instance = null;
		private static Juice Instance {
			get{if (instance == null) instance = Create(); return instance;}
		}
		private Juice() {
		}
		private static Juice Create() {
			GameObject go = new GameObject("JuiceDriver");
			return go.AddComponent<Juice>();
		}
		#endregion
	
		#region Predefined curves
		public static AnimationCurve SproingIn {
			get {
				return new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.7f, 1.1f), new Keyframe(0.85f, 0.9f), new Keyframe(1,1));
			}
		}
		public static AnimationCurve FastFalloff {
			get {
	            return new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(0.25f, 0.8f, 1, 1), new Keyframe(1, 1));
			}
		}
		public static AnimationCurve LateFalloff {
			get {
				return new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.75f, 0.25f), new Keyframe(1, 1));
			}
		}
		public static AnimationCurve Wobble {
			get {
				return new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.25f, 1), new Keyframe(0.75f, -1), new Keyframe(1, 0));
			}
		}
		public static AnimationCurve Linear {
			get {
				return new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
			}
		}
		public static AnimationCurve Hop {
			get {
				return new AnimationCurve(new Keyframe(0,0), new Keyframe(0.5f, 1), new Keyframe(1,0));
			}
		}
		public static AnimationCurve SharpHop {
			get {
				return new AnimationCurve(new Keyframe(0,1), new Keyframe(1,0));
			}
		}
		#endregion
	
		#region Fields
		public List<JuiceData     > list      = new List<JuiceData     >();
		public List<JuiceDataColor> listColor = new List<JuiceDataColor>();
		float savedTimescale;
		float sleepTimer;
		bool sleep;
		#endregion
	
		#region MonoBehaviour methods
		private void Update() {
			for (int i = 0; i < list.Count; i++) {
				bool remove = list[i].Update();
	
				if (remove) {
					if (list[i].callback != null) list[i].callback();
					list.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < listColor.Count; i++) {
				bool remove = listColor[i].Update();
	
				if (remove) {
					if (listColor[i].callback != null) listColor[i].callback();
					listColor.RemoveAt(i);
					i--;
				}
			}
			
			if (sleep && Time.realtimeSinceStartup > sleepTimer) {
				sleep=false;
				Time.timeScale = savedTimescale;
			}
		}
		#endregion
	
		#region Static interface stuff
		public static void Add (Transform aTransform, JuiceType aType, AnimationCurve aCurve, float aStart = 0, float aEnd = 1, float aDuration = 1, bool aRelative = false, Action aCallback = null) {
			JuiceData data = new JuiceData();
			data.transform = aTransform;
			data.type      = aType ;
			data.curve     = aCurve;
			data.start     = aStart;
			data.duration  = aDuration;
			data.startTime = Time.time;
			data.end       = aEnd;
			data.relative  = aRelative;
			data.callback  = aCallback;
			Instance.list.Add (data);
			data.Update();
		}
		public static void Scale(Transform aTransform, AnimationCurve aCurve, float aStart = 0, float aEnd = 1, float aDuration = 1, bool aRelative = false, Action aCallback = null) {
			Add (aTransform, JuiceType.ScaleXYZ, aCurve, aStart, aEnd, aDuration, aRelative, aCallback);
		}
	    public static void Scale(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration = 1, bool aRelative = false, Action aCallback = null) {
	        Add(aTransform, JuiceType.ScaleX, aCurve, aStart.x, aEnd.x, aDuration, aRelative, aCallback);
	        Add(aTransform, JuiceType.ScaleY, aCurve, aStart.y, aEnd.y, aDuration, aRelative, aCallback);
	        Add(aTransform, JuiceType.ScaleZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
	    }
	    public static void Rotate(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration = 1, bool aRelative = false, Action aCallback = null) {
	        Add(aTransform, JuiceType.RotationX, aCurve, aStart.x, aEnd.x, aDuration, aRelative, aCallback);
	        Add(aTransform, JuiceType.RotationY, aCurve, aStart.y, aEnd.y, aDuration, aRelative, aCallback);
	        Add(aTransform, JuiceType.RotationZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
	    }
		public static void Translate(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration, bool aRelative = false, Action aCallback = null) {
	        Vector3 parentPos = Vector3.zero;
	        if (aTransform.parent != null)
	            parentPos = aTransform.parent.position;
	
	        Add(aTransform, JuiceType.TranslateX, aCurve, aStart.x - parentPos.x, aEnd.x - parentPos.x, aDuration, aRelative);
	        Add(aTransform, JuiceType.TranslateY, aCurve, aStart.y - parentPos.y, aEnd.y - parentPos.y, aDuration, aRelative);
	        Add(aTransform, JuiceType.TranslateZ, aCurve, aStart.z - parentPos.z, aEnd.z - parentPos.z, aDuration, aRelative, aCallback);
		}
	    public static void TranslateLocal(Transform aTransform, AnimationCurve aCurve, Vector3 aStart, Vector3 aEnd, float aDuration, bool aRelative = false, Action aCallback = null) {
	        Add(aTransform, JuiceType.TranslateX, aCurve, aStart.x, aEnd.x, aDuration, aRelative);
	        Add(aTransform, JuiceType.TranslateY, aCurve, aStart.y, aEnd.y, aDuration, aRelative);
	        Add(aTransform, JuiceType.TranslateZ, aCurve, aStart.z, aEnd.z, aDuration, aRelative, aCallback);
	    }
		public static void Color(Material aRenderer, AnimationCurve aCurve, Color aStart, Color aEnd, float aDuration, Action aCallback = null) {
			JuiceDataColor data = new JuiceDataColor();
			data.renderer  = aRenderer;
			data.curve     = aCurve;
			data.start     = aStart;
			data.duration  = aDuration;
			data.startTime = Time.time;
			data.end       = aEnd;
			data.callback  = aCallback;
			Instance.listColor.Add (data);
			data.Update();
		}
		public static void Cancel(Transform aTransform, bool aFinishEffect = true) {
			for (int i=0;i<Instance.list.Count;i+=1) {
				if (Instance.list[i].transform == aTransform) {
	                if (aFinishEffect)
					    Instance.list[i].Cancel();
					Instance.list.RemoveAt(i);
					i--;
				}
			}
		}
		public static void Cancel(Renderer aRenderer, bool aFinishEffect = true) {
			for (int i=0;i<Instance.listColor.Count;i+=1) {
				if (Instance.listColor[i].renderer == aRenderer) {
	                if (aFinishEffect)
					    Instance.listColor[i].Cancel();
					Instance.listColor.RemoveAt(i);
					i--;
				}
			}
		}
		public static void SlowMo(float aSpeed) {
			Time.timeScale      = aSpeed;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}
		public static void Sleep(float aDuration) {
			Instance.savedTimescale = Time.timeScale == 0 ? Instance.savedTimescale : Time.timeScale;
			Time.timeScale      = 0;
			Instance.sleep = true;
			Instance.sleepTimer = Time.realtimeSinceStartup + aDuration;
		}
		public static void SleepMS(int aMilliseconds) {
			Sleep (aMilliseconds * 0.001f);
		}
		#endregion
	}
}