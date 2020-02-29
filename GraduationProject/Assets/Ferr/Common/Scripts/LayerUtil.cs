using UnityEngine;

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

using System.Collections;
using System.Reflection;
#endif

namespace Ferr {
	public static class RuntimeLayerUtil {
		static List<int> mReservedLayers = null;
		
		public static int  GetFreeLayer() {
			for (int i = 16; i < 32; i += 1) {
				if (LayerMask.LayerToName(i) == "" && (mReservedLayers == null || !mReservedLayers.Contains(i)))
					return i;
			}
			Debug.LogError("Ferr is looking for an unnamed render layer after 15, but none are free!");
			return -1;
		}
		public static void ReserveLayer(int aLayerID) {
			if (mReservedLayers == null)
				mReservedLayers = new List<int>();
			mReservedLayers.Add(aLayerID);
		}
	}
	#if UNITY_EDITOR
	public static class LayerUtil {
		static SerializedObject GetLayerManager() {
			return new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		}
		static SerializedProperty[] GetLayers(SerializedObject aLayerManager) {
			#if UNITY_5_3_OR_NEWER
			SerializedProperty layers = aLayerManager.FindProperty("layers");
			#endif
			SerializedProperty[] result = new SerializedProperty[32];
			
			for (int i=0; i<32; i+=1) {
				#if UNITY_5_3_OR_NEWER
				SerializedProperty property = layers.GetArrayElementAtIndex(i);
				#else
				string             name     = i < 8 ? "Builtin Layer "+i : "User Layer "+i;
				SerializedProperty property = aLayerManager.FindProperty(name);
				#endif
				result[i] = property;
			}
			
			return result;
		}
		public static int GetOrCreateLayer(string aName) {
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			for (int i = 0; i < layers.Length; i+=1) {
				if (layers[i].stringValue == aName) {
					return i;
				}
			}
			
			for (int i = 8; i < layers.Length; i+=1) {
				if (layers[i].stringValue == "") {
					layers[i].stringValue = aName;
					lMan.ApplyModifiedProperties();
					return i;
				}
			}
			
			return -1;
		}
		public static string GetLayerName(int aIndex) {
			if (aIndex < 0 || aIndex >= 32) return "";
			
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			return layers[aIndex].stringValue;
		}
		public static void SetLayerName(int aIndex, string aName) {
			if (aIndex < 8 || aIndex >= 32) return;
			
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			layers[aIndex].stringValue = aName;
			lMan.ApplyModifiedProperties();
		}
		public static int GetFirstUnnamedUserLayer() {
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			for (int i = 8; i < layers.Length; i+=1) {
				if (layers[i].stringValue == "") {
					return i;
				}
			}
			return -1;
		}
		
		public static string[] GetSortingLayerNames() {
			Type utility = Type.GetType("UnityEditorInternal.InternalEditorUtility, UnityEditor");
			if (utility == null) return null;
			
			PropertyInfo sortingLayerNames = utility.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			if (sortingLayerNames == null) return null;
			
			return sortingLayerNames.GetValue(null, null) as string[];
		}
	}
	#endif
}