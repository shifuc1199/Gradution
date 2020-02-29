using UnityEngine;
using System;
using System.Collections;

namespace Ferr {
	
	public class WebMessager : MonoBehaviour {
		private static WebMessager mInstance = null;
		public  static WebMessager Instance {
			get {
				if (mInstance == null) {
					GameObject go = new GameObject("_WebMessager");
					mInstance = go.AddComponent<WebMessager>();
				}
				return mInstance;
			}
		}
		int activeMessages = 0;
		
		public event Action OnAllMessagesComplete;
	
		void Start () {
			DontDestroyOnLoad(gameObject);
		}
		
		public void Post    (string aTo, byte[] aData,  Action<WWW>     aCallback, Action<WWW>    aOnError) {
			WWWForm form = new WWWForm();
			form.AddBinaryData("body", aData);
			
			StartCoroutine(Send (aTo, form, aCallback, aOnError));
		}
		public void Post    (string aTo, string aData,  Action<WWW>     aCallback, Action<WWW>    aOnError) {
			byte[] bytes = new byte[aData.Length];
			for (int i = 0; i < bytes.Length; ++i) bytes[i] = (byte)aData[i];
			
			Post(aTo, bytes, aCallback, aOnError);
		}
		public void PostForm(string aTo, WWWForm aForm, Action<WWW>     aCallback, Action<WWW>    aOnError) {
			StartCoroutine(Send (aTo, aForm, aCallback, aOnError));
		}
		
		public void GetText    (string aTo,                Action<string>  aCallback, Action<WWW> aOnError) {
			StartCoroutine(Send (aTo, aCallback, aOnError));
		}
		public void GetRaw     (string aTo,                Action<WWW>     aCallback, Action<WWW> aOnError) {
			StartCoroutine(Send (aTo, aCallback, aOnError));
		}
		public void GetImage   (string aTo,                Action<Texture> aCallback, Action<WWW> aOnError) {
			StartCoroutine(Send (aTo, aCallback, aOnError));
		}
		
		private IEnumerator Send(string aTo, Action<WWW> aCallback, Action<WWW> aOnError) {
			BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			
			if (String.IsNullOrEmpty(www.error) && aCallback != null) {
				aCallback(www);
			} else if (!String.IsNullOrEmpty(www.error) && aOnError != null) {
				aOnError (www);
			}
			FinishMessage();
		}
		private IEnumerator Send(string aTo, Action<string> aCallback, Action<WWW> aOnError) {
			BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			
			if (String.IsNullOrEmpty(www.error) && aCallback != null) {
				aCallback(www.text);
			} else if (!String.IsNullOrEmpty(www.error) && aOnError != null) {
				aOnError(www);
			}
			FinishMessage();
		}
		
		private IEnumerator Send(string aTo, Action<Texture> aCallback, Action<WWW> aOnError) {
			BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			
			if (String.IsNullOrEmpty(www.error) && aCallback != null) {
				aCallback(www.texture);
			} else if (!String.IsNullOrEmpty(www.error) && aOnError != null) {
				aOnError(www);
			}
			FinishMessage();
		}
		private IEnumerator Send(string aTo, WWWForm aForm, Action<WWW> aCallback, Action<WWW> aOnError) {
			BeginMessage();
			WWW www = new WWW(aTo, aForm);
			yield return www;
			
			if (String.IsNullOrEmpty(www.error) && aCallback != null) {
				aCallback(www);
			}else if (!String.IsNullOrEmpty(www.error) && aOnError != null) {
				aOnError(www);
			}
			FinishMessage();
		}
		private IEnumerator Send(string aTo, byte[] aData, Action<WWW> aCallback, Action<WWW> aOnError) {
			BeginMessage();
			WWW www = new WWW(aTo, aData);
			yield return www;
			
			if (String.IsNullOrEmpty(www.error) && aCallback != null) {
				aCallback(www);
			}else if (!String.IsNullOrEmpty(www.error) && aOnError != null) {
				aOnError(www);
			}
			FinishMessage();
		}
	
	    public int GetActive() {
	        return activeMessages;
	    }
		private void BeginMessage() {
			activeMessages += 1;
		}
		private void FinishMessage() {
			activeMessages -= 1;
			if (activeMessages == 0 && OnAllMessagesComplete != null) {
				OnAllMessagesComplete();
			}
		}
	}
}