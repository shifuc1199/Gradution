using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr.Example {
	public enum Side {
		Left,
		Right
	}
	public class InfiniteTerrain : MonoBehaviour {
		[SerializeField] Transform              _trackObject = null;
		[SerializeField] InfiniteTerrainChunk[] _terrainPrefabs = null;
		[SerializeField] float[]                _terrainPrefabWeights;
		[SerializeField] float                  _generateDistanceRight = 10;
		[SerializeField] float                  _generateDistanceLeft  = 10;
		
		float                                                        _weightSum;
		List<InfiniteTerrainChunk>                                   _activeInstances = new List<InfiniteTerrainChunk>();
		Dictionary<InfiniteTerrainChunk, List<InfiniteTerrainChunk>> _terrainPool = new Dictionary<InfiniteTerrainChunk, List<InfiniteTerrainChunk>>();

		private void Awake() {
			_weightSum = GetWeightSum();

			// fill in the pool with some initial items
			for (int i = 0; i < _terrainPrefabs.Length; i++) {
				GetFreePrefabInstance(_terrainPrefabs[i]);
			}
		}

		private void Start() {
			Update();
		}

		private void Update() {
			AddInstances();
			ClipInstances();
		}

		void AddInstances() {
			int     max         = 2;
			Vector3 left        = GetLeftmost();
			float   desiredLeft = _trackObject.position.x - _generateDistanceLeft;
			while (left.x > desiredLeft && max > 0) {
				AddNewChunk(left, Side.Left);
				max -= 1;
				left = GetLeftmost();
			}

			max = 2;
			Vector3 right       = GetRightmost();
			float   desiredRight = _trackObject.position.x + _generateDistanceRight;
			while (right.x < desiredRight && max > 0) {
				AddNewChunk(right, Side.Right);
				max -= 1;
				right = GetRightmost();
			}
		}

		Vector3 GetLeftmost() {
			Vector3 result = Vector3.zero;
			for (int i = 0; i < _activeInstances.Count; i++) {
				if (_activeInstances[i].LeftHook.x < result.x)
					result = _activeInstances[i].LeftHook;
			}
			return result;
		}
		Vector3 GetRightmost() {
			Vector3 result = Vector3.zero;
			for (int i = 0; i < _activeInstances.Count; i++) {
				if (_activeInstances[i].RightHook.x > result.x)
					result = _activeInstances[i].RightHook;
			}
			return result;
		}

		void ClipInstances() {
			float leftX  = _trackObject.position.x - _generateDistanceLeft;
			float rightX = _trackObject.position.x + _generateDistanceRight;
			for (int i=_activeInstances.Count-1; i>=0; i-=1) {
				if (_activeInstances[i].RightHook.x < leftX || _activeInstances[i].LeftHook.x > rightX) {
					_activeInstances[i].gameObject.SetActive(false);
					_activeInstances.RemoveAt(i);
				}
			}
		}

		void AddNewChunk(Vector3 aTo, Side aSide) {
			InfiniteTerrainChunk prefab   = GetRandomPrefab();
			InfiniteTerrainChunk instance = GetFreePrefabInstance(prefab);
			
			instance.ConnectTo(aTo, aSide);
			instance.gameObject.SetActive(true);
			_activeInstances.Add(instance);
		}

		InfiniteTerrainChunk GetFreePrefabInstance(InfiniteTerrainChunk aPrefab) {
			List<InfiniteTerrainChunk> instances;
			// ensure the pool has a list available for this prefab
			if (!_terrainPool.TryGetValue(aPrefab, out instances)) {
				instances = new List<InfiniteTerrainChunk>();
				_terrainPool.Add(aPrefab, instances);
			}

			// now make sure there's an available instance in the list
			for (int i = 0; i < instances.Count; i++) {
				if (!instances[i].gameObject.activeSelf)
					return instances[i];
			}

			// no instance was found, go ahead and create one
			GameObject obj = Instantiate(aPrefab.gameObject);
			InfiniteTerrainChunk chunk = obj.GetComponent<InfiniteTerrainChunk>();
			obj.SetActive(false);
			instances.Add(chunk);
			return chunk;
		}

		InfiniteTerrainChunk GetRandomPrefab() {
			float random = UnityEngine.Random.value * _weightSum;

			float curr = 0;
			for (int i = 0; i < _terrainPrefabWeights.Length; i++) {
				curr += _terrainPrefabWeights[i];
				if (random <= curr)
					return _terrainPrefabs[i];
			}

			return null;
		}

		float GetWeightSum() {
			float result = 0;
			for (int i = 0; i < _terrainPrefabWeights.Length; i++) {
				result += _terrainPrefabWeights[i];
			}
			return result;
		}

		private void OnValidate() {
			// ensure weights stay in sync with prefabs
			if (_terrainPrefabWeights.Length != _terrainPrefabs.Length)
				Array.Resize(ref _terrainPrefabWeights, _terrainPrefabs.Length);

			// update this in case we're editing values during runtime
			_weightSum = GetWeightSum();
		}
	}
}