using System;
using System.Collections;
using UnityEngine;

public class MyMaths
{
	public static float Percentage(float current, float max)
	{
		return current / max;
	}
	
	/// <summary>
	/// Takes a list of gameobjects and sorts them alphabetically. Sorts by ascending as a defualt
	/// </summary>
	public static GameObject[] SortAlphabetically(GameObject[] input)
	{
		return SortAlphabetically(input, true);
	}
	
	/// <summary>
	/// Takes a list of gameobjects and sorts them alphabetically.
	/// </summary>
	/// <param name="input">Your gameobject array</param>
	/// <param name="sortAscending">If the list should be A>Z or Z>A</param>
	/// <returns></returns>
	public static GameObject[] SortAlphabetically(GameObject[] input, bool sortAscending)
	{
		string[] names = new string[input.Length];
		GameObject[] output = new GameObject[input.Length];
		
		for (int i = 0; i < input.Length; i++)
			names[i] = input[i].name;
		
		Array.Sort(names);
		
		if (!sortAscending)
			Array.Reverse(names);
		
		for (int i = 0; i < input.Length; i++)
			for (int j = 0; j < input.Length; j++)
				if (names[i].Equals(input[j].name))
			{
				output[i] = input[j];
				break;
			}
		
		return output;
	}
}
