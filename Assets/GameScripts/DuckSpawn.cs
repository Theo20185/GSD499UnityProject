using UnityEngine;
using System.Collections;

public class DuckSpawn : MonoBehaviour 
{
	public enum DuckType
	{
		Easy, Hard
	}

	public int ducksNeeded;
	public DuckType duckType;
	public int spawnTime;
	public int ducksPerSpawn;
	public int spawnSpanX;
	public int spawnSpanY;
	public int spawnSpanZ;
}
