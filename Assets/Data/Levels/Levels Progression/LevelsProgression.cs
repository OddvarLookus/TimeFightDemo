using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(menuName = "Levels Progression")]
public class LevelsProgression : SerializedScriptableObject
{
	public List<List<Level>> runLevels = new List<List<Level>>();
	
	public Level GetLevelAt(int idx)
	{
		int maxIdx = runLevels[idx].Count;
		int randIdx = Random.Range(0, maxIdx);
		Debug.Log($"idx: {idx}, rand: {randIdx}");
		return runLevels[idx][randIdx];
	}
	
}
