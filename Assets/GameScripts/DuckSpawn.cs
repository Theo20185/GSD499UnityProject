using UnityEngine;
using System.Collections;

public class DuckSpawn : MonoBehaviour 
{
	public enum DuckType
	{
		Easy, Hard, Clay
	}

	public int ducksNeeded;
	public DuckType duckType;
	public int escapeTime;
	public int ducksPerSpawn;
	public int spawnRounds;
	public Vector3 spawnSpan;
}
