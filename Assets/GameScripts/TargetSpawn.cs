using UnityEngine;
using System.Collections;

public class TargetSpawn : MonoBehaviour 
{
	public enum TargetType
	{
		EasyDuck, HardDuck, Clay
	}

	public int targetsNeeded;
	public TargetType targetType;
	public int escapeTime;
	public int targetsPerSpawn;
	public int spawnRounds;
	public Vector3 spawnSpan;
}
