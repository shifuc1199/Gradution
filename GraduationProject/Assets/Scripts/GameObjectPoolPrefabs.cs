/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameObjectPoolPrefabs")]
public class GameObjectPoolPrefabs : ScriptableObject 
{
    public List<GameObjectPoolPrefab> Prefabs = new List<GameObjectPoolPrefab>();
}
[System.Serializable]
public class GameObjectPoolPrefab
{
    public string prefab_name;
    public GameObject prefab;
}

